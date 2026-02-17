using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TurekSimulator
{
	public partial class MainForm : Form
	{
		private GameManager _game;
		private readonly MenuForm _menu;

		// ===== Patience timer (logika kolejki) =====
		// Timer działa co PatienceTimerMs, ale realna utrata cierpliwości liczona jest z czasu (elapsedSeconds),
		// dzięki czemu nie "przyspiesza" przy chwilowych przycinkach UI.
		private Timer _patienceTimer;
		private const int PatienceTimerMs = 100;                 // częstotliwość odświeżania logiki (10x/s)
		private DateTime _lastPatienceTickUtc = DateTime.UtcNow; // znacznik czasu ostatniego kroku logiki

		// Anti-reentrancy: nie pozwala, żeby Tick wchodził drugi raz zanim pierwszy skończy (np. UI lag).
		private bool _patienceTicking = false;

		// Pauza po wejściu do menu (ESC)
		private bool _pausedByMenu = false;

		// Zapamiętanie wyboru w tabeli klientów – żeby RefreshUi() nie zjadał selekcji
		private int _desiredCustomerIndex = -1;

		// ===== Klienci – sprite + stałe imię =====
		private readonly Random _rng = new Random();
		private readonly Dictionary<Customer, Image> _customerSprites = new Dictionary<Customer, Image>();
		private readonly Dictionary<Customer, string> _customerDisplayNames = new Dictionary<Customer, string>();

		private class SpriteEntry
		{
			public Image Img;
			public string Name;
			public bool Male;
			public SpriteEntry(Image img, string name, bool male)
			{
				Img = img;
				Name = name;
				Male = male;
			}
		}

		private SpriteEntry[] _spriteEntries;

		// ===== Animowana kolejka (wizualizacja) =====
		private class QueueVisual
		{
			public Customer Customer;
			public Image Img;
			public float X;
			public float TargetX;

			public QueueVisual(Customer c, Image img, float x, float targetX)
			{
				Customer = c;
				Img = img;
				X = x;
				TargetX = targetX;
			}
		}

		private readonly List<QueueVisual> _queueVisuals = new List<QueueVisual>();
		private Timer _animTimer;

		// Tło panelu kolejki
		private Image _queueBg;

		// Stały rozmiar sprite
		private const int SpriteW = 128;
		private const int SpriteH = 256;
		private const int MaxVisible = 6;

		// Ustawienia kolejki
		private int _spacing = 80;
		private int _floorYOffset = 36;

		// Animacja przesuwania
		private float _lerp = 0.28f;
		private int _rightMargin = 40;

		// Fallback (gdybyś odpalił bez menu)
		public MainForm() : this(new GameManager(), null) { }

		/// <summary>
		/// Główny konstruktor okna gry. Inicjalizuje UI, wczytuje sprity oraz uruchamia timery:
		/// - _animTimer: płynne przesuwanie sprite'ów w kolejce,
		/// - _patienceTimer: logika spadku cierpliwości klientów.
		/// </summary>
		public MainForm(GameManager gm, MenuForm menu)
		{
			InitializeComponent();

			_game = gm;
			_menu = menu;

			// Muzyka ambientowa w tle (best-effort)
			try { AudioManager.StartAmbientLoop(); } catch { }

			// Styl HUD / pixel theme
			ApplyPixelHudTheme();

			// Zmniejszenie migotania (double buffering)
			EnableDoubleBuffering(this);
			EnableDoubleBuffering(pnlQueueView);

			// Przeniesienie tła panelu kolejki do własnego rysowania (mniej flicker)
			_queueBg = pnlQueueView.BackgroundImage;
			pnlQueueView.BackgroundImage = null;
			if (_queueBg == null) pnlQueueView.BackColor = Color.Black;

			// Sprity + stałe imiona klientów
			LoadCustomerSprites();
			AssignSpritesToTodayCustomers();

			// Rysowanie kolejki i wybór klienta kliknięciem
			pnlQueueView.Paint += pnlQueueView_Paint;
			pnlQueueView.MouseClick += pnlQueueView_MouseClick;
			pnlQueueView.Resize += (s, e) => RefreshQueueTargets();

			// Timer animacji sprite'ów kolejki
			_animTimer = new Timer();
			_animTimer.Interval = 16; // ~60 FPS
			_animTimer.Tick += AnimTimer_Tick;
			_animTimer.Start();

			// Timer logiki cierpliwości
			_patienceTimer = new Timer();
			_patienceTimer.Interval = PatienceTimerMs;
			_patienceTimer.Tick += PatienceTimer_Tick;
			_lastPatienceTickUtc = DateTime.UtcNow;
			_patienceTimer.Start();

			// Obsługa ESC -> menu
			this.KeyPreview = true;
			this.KeyDown += MainForm_KeyDown;

			// Zapamiętanie selekcji użytkownika w tabeli (żeby RefreshUi nie cofał na pierwszy wiersz)
			dgvCustomers.SelectionChanged += (s, e) =>
			{
				if (dgvCustomers.CurrentRow != null)
					_desiredCustomerIndex = dgvCustomers.CurrentRow.Index;
			};

			// Gdy okno wraca z menu / staje się widoczne, resetujemy znacznik czasu,
			// aby nie "nadganiać" cierpliwości po pauzie.
			this.VisibleChanged += (s, e) =>
			{
				if (this.Visible)
				{
					_lastPatienceTickUtc = DateTime.UtcNow;
					ResumeTimersIfNeeded();
				}
			};

			this.Activated += (s, e) =>
			{
				ResumeTimersIfNeeded();
			};

			RefreshUi();
		}

		/// <summary>
		/// Zatrzymuje timery rozgrywki (cierpliwość + animacja).
		/// Używane wyłącznie do pauzy w trakcie okna podsumowania dnia.
		/// </summary>
		private void StopGameplayTimers()
		{
			try { _patienceTimer?.Stop(); } catch { }
			try { _animTimer?.Stop(); } catch { }
		}

		/// <summary>
		/// Wznawia timery rozgrywki (cierpliwość + animacja) oraz resetuje znacznik czasu,
		/// żeby po pauzie nie było skoku elapsedSeconds.
		/// </summary>
		private void StartGameplayTimers()
		{
			_lastPatienceTickUtc = DateTime.UtcNow;

			try { if (_animTimer != null && !_animTimer.Enabled) _animTimer.Start(); } catch { }
			try { if (_patienceTimer != null && !_patienceTimer.Enabled) _patienceTimer.Start(); } catch { }
		}

		/// <summary>
		/// Odświeża elementy HUD (dzień, kasa, reputacja) oraz tabele: magazyn i klienci.
		/// Uwaga: DataSource jest podmieniany, więc na końcu przywracamy selekcję klienta.
		/// </summary>
		private void RefreshUi()
		{
			// Zachowaj wybór przed rebindingiem (SelectionChanged może nie zdążyć)
			int prevIndex = _desiredCustomerIndex;
			if (dgvCustomers.CurrentRow != null)
				prevIndex = dgvCustomers.CurrentRow.Index;

			this.SuspendLayout();
			dgvCustomers.SuspendLayout();
			dgvInventory.SuspendLayout();

			lblDay.Text = $"Dzień: {_game.Day}";
			lblCash.Text = $"Gotówka: {_game.Player.Cash:C}";
			lblReputation.Text = $"Reputacja: {_game.Player.Reputation}";

			dgvInventory.DataSource = _game.Player.Inventory.Items
				.Select(i => new
				{
					Składnik = i.Ingredient.Name,
					Ilość = i.Quantity
				})
				.ToList();

			// Pokazujemy realny stan: PatienceLeft
			dgvCustomers.DataSource = _game.CustomersToday
				.Select(c => new
				{
					Imię = _customerDisplayNames.ContainsKey(c) ? _customerDisplayNames[c] : c.Name,
					Cierpliwość = c.PatienceLeft,
					Zapłaci = c.PriceToPay
				})
				.ToList();

			dgvCustomers.AutoResizeColumns();
			dgvInventory.AutoResizeColumns();

			ColorCustomersByPatience();

			dgvInventory.ResumeLayout();
			dgvCustomers.ResumeLayout();
			this.ResumeLayout();

			RefreshQueueTargets();

			// Przywróć selekcję po podmianie DataSource
			_desiredCustomerIndex = prevIndex;
			RestoreCustomerSelection();
		}

		/// <summary>
		/// Przywraca selekcję w DataGridView po odświeżeniu (podmianie DataSource),
		/// aby wybór użytkownika nie był nadpisywany przez RefreshUi().
		/// </summary>
		private void RestoreCustomerSelection()
		{
			if (dgvCustomers.Rows.Count == 0) return;
			if (_desiredCustomerIndex < 0) return;

			int idx = _desiredCustomerIndex;
			if (idx >= dgvCustomers.Rows.Count) idx = dgvCustomers.Rows.Count - 1;

			dgvCustomers.ClearSelection();
			dgvCustomers.Rows[idx].Selected = true;
			dgvCustomers.CurrentCell = dgvCustomers.Rows[idx].Cells[0];
		}

		/// <summary>
		/// Koloruje wiersze klientów wg procentu pozostałej cierpliwości.
		/// </summary>
		private void ColorCustomersByPatience()
		{
			if (dgvCustomers.Rows.Count == 0) return;

			for (int i = 0; i < _game.CustomersToday.Count && i < dgvCustomers.Rows.Count; i++)
			{
				var c = _game.CustomersToday[i];
				var row = dgvCustomers.Rows[i];

				double ratio = (c.Patience <= 0) ? 0 : (double)c.PatienceLeft / c.Patience;

				if (ratio > 0.60)
					row.DefaultCellStyle.ForeColor = Color.LightGreen;
				else if (ratio > 0.30)
					row.DefaultCellStyle.ForeColor = Color.Orange;
				else
					row.DefaultCellStyle.ForeColor = Color.IndianRed;
			}
		}

		/// <summary>
		/// Dopisuje komunikat do logu w oknie gry.
		/// </summary>
		private void Log(string message)
		{
			txtLog.AppendText(message + Environment.NewLine);
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				ShowMenuFromGame();
				e.Handled = true;
			}
		}

		/// <summary>
		/// Wchodzi do menu pauzy. Zatrzymuje timery, ukrywa okno gry i pokazuje MenuForm w trybie pauzy.
		/// </summary>
		private void ShowMenuFromGame()
		{
			if (_menu == null) return;

			_pausedByMenu = true;

			try { _patienceTimer.Stop(); } catch { }
			try { _animTimer.Stop(); } catch { }

			this.Hide();
			_menu.ShowAsPause(this, _game);
		}

		/// <summary>
		/// Wznawia grę po wyjściu z menu pauzy (wywoływane przez MenuForm).
		/// </summary>
		public void ResumeFromMenu()
		{
			_pausedByMenu = false;
			_lastPatienceTickUtc = DateTime.UtcNow;

			this.Show();
			this.BringToFront();
			this.Activate();

			try { _animTimer.Start(); } catch { }
			try { _patienceTimer.Start(); } catch { }
		}

		/// <summary>
		/// Jednolity wrapper na okienko dialogowe w stylu gry.
		/// </summary>
		private void ShowGameDialog(string title, string message, bool danger = false)
		{
			try
			{
				using (var dlg = new GameDialogForm(title, message, danger))
				{
					dlg.StartPosition = FormStartPosition.CenterParent;
					dlg.ShowDialog(this);
				}
			}
			catch
			{
				MessageBox.Show(message, title);
			}
		}

		private void PauseGameTimers()
		{
			try { _patienceTimer?.Stop(); } catch { }
			try { _animTimer?.Stop(); } catch { }
		}

		private void ResumeGameTimers()
		{
			_lastPatienceTickUtc = DateTime.UtcNow; // żeby nie "nadganiało" po pauzie

			try { if (_animTimer != null && !_animTimer.Enabled) _animTimer.Start(); } catch { }
			try { if (_patienceTimer != null && !_patienceTimer.Enabled) _patienceTimer.Start(); } catch { }
		}

		private void GoToMenuAfterGameOver()
		{
			PauseGameTimers();

			// możesz tu ewentualnie zrobić reset gry, jeśli masz w menu "Nowa gra"
			this.Hide();

			if (_menu != null)
			{
				_menu.Show();
				_menu.BringToFront();
				_menu.Activate();
			}
		}


		/// <summary>
		/// Tick logiki cierpliwości klientów. Zastosowane zabezpieczenia:
		/// - anti-reentrancy (brak nakładania Ticków),
		/// - time-based update (elapsedSeconds) – brak "przyspieszania" po lagach UI,
		/// - brak popupów podczas odejścia klientów (tylko log).
		/// </summary>
		private void PatienceTimer_Tick(object sender, EventArgs e)
		{
			if (_pausedByMenu) return;
			if (_patienceTicking) return;

			_patienceTicking = true;
			try
			{
				var now = DateTime.UtcNow;

				// Wyliczamy realny czas od ostatniego kroku logiki
				double elapsedSeconds = (now - _lastPatienceTickUtc).TotalSeconds;
				if (elapsedSeconds <= 0) return;

				_lastPatienceTickUtc = now;

				var leavers = _game.TickPatienceAndGetLeavers(elapsedSeconds);
				if (leavers != null && leavers.Count > 0)
				{
					foreach (var l in leavers)
					{
						_customerSprites.Remove(l);
						_customerDisplayNames.Remove(l);
						_queueVisuals.RemoveAll(v => ReferenceEquals(v.Customer, l));
					}

					Log($"⏳ {leavers.Count} klient(ów) odszedł(o) przez czekanie. Reputacja spadła.");
				}

				RefreshUi();
			}
			finally
			{
				_patienceTicking = false;
			}
		}

		/// <summary>
		/// Zakup składników. Błędy (np. brak gotówki) pokazujemy w oknie dialogowym gry.
		/// </summary>
		private void BuyIngredient(IngredientType type, int amount)
		{
			try
			{
				_game.BuyIngredient(type, amount);
				Log($"Kupiono: {amount} x {type}.");
				RefreshUi();
			}
			catch (Exception ex)
			{
				ShowGameDialog("Błąd sklepu", ex.Message);
			}
		}

		private void btnBuyBread_Click(object sender, EventArgs e) => BuyIngredient(IngredientType.Bread, 10);
		private void btnBuyMeat_Click(object sender, EventArgs e) => BuyIngredient(IngredientType.Meat, 10);
		private void btnBuyVeggies_Click(object sender, EventArgs e) => BuyIngredient(IngredientType.Veggies, 10);
		private void btnBuySauce_Click(object sender, EventArgs e) => BuyIngredient(IngredientType.Sauce, 10);

		/// <summary>
		/// Obsługa wybranego klienta: walidacja wyboru, próba obsługi w GameManager,
		/// a przy sukcesie usunięcie klienta z kolejki oraz odświeżenie UI.
		/// </summary>
		private void btnServeSelected_Click(object sender, EventArgs e)
		{
			if (dgvCustomers.CurrentRow == null)
			{
				ShowGameDialog("Brak wyboru", "Najpierw wybierz klienta.");
				return;
			}

			int rowIndex = dgvCustomers.CurrentRow.Index;
			if (rowIndex < 0 || rowIndex >= _game.CustomersToday.Count)
			{
				ShowGameDialog("Błąd", "Nieprawidłowy wybór klienta.");
				return;
			}

			var customer = _game.CustomersToday[rowIndex];

			bool ok = _game.TryServeCustomer(customer, out string message);
			Log(message);

			if (!ok)
			{
				ShowGameDialog("Nie można obsłużyć", message);
				RefreshUi();
				return;
			}

			_game.CustomersToday.RemoveAt(rowIndex);

			_customerSprites.Remove(customer);
			_customerDisplayNames.Remove(customer);
			_queueVisuals.RemoveAll(v => ReferenceEquals(v.Customer, customer));

			_desiredCustomerIndex = Math.Min(rowIndex, _game.CustomersToday.Count - 1);

			RefreshUi();
		}

		/// <summary>
		/// Przycisk testowego eventu (debug / mechanika losowych zdarzeń).
		/// </summary>
		private void btnTriggerEvent_Click(object sender, EventArgs e)
		{
			var ev = _game.TryTriggerEvent();
			Log(ev == null ? "Dziś nic się nie wydarzyło." : $"{ev.Title}: {ev.Description}");
			RefreshUi();
		}

		/// <summary>
		/// Kończy dzień: zatrzymuje timery na czas podsumowania (żeby nowy dzień nie leciał w tle),
		/// a po zamknięciu okna wznawia timery i resetuje znacznik czasu.
		/// </summary>
		private void btnEndDay_Click(object sender, EventArgs e)
		{
			// Podsumowanie ma być czytelne -> timery STOP na czas okna
			PauseGameTimers();

			string report = _game.EndDayAndGetReport();
			Log("=== KONIEC DNIA ===");
			Log(report);

			ShowGameDialog("Podsumowanie dnia", report);

			// Jeśli po zakończeniu dnia gra została przegrana -> pokaż przegraną i wróć do menu
			if (_game.IsGameOver)
			{
				ShowGameDialog("Przegrana", "Zdenerwowani kibole roznieśli twoją budkę.", danger: true);
				GoToMenuAfterGameOver();
				return;
			}

			// Normalnie: generacja nowej kolejki już zaszła w GameManager (o ile nie było game over)
			AssignSpritesToTodayCustomers();
			_queueVisuals.Clear();

			_desiredCustomerIndex = -1;
			RefreshUi();

			// Po zamknięciu okna podsumowania wznawiamy timery
			ResumeGameTimers();
		}


		/// <summary>
		/// Wczytuje listę dostępnych sprite'ów klientów wraz z przypisanymi imionami.
		/// </summary>
		private void LoadCustomerSprites()
		{
			_spriteEntries = new SpriteEntry[]
			{
				new SpriteEntry(Properties.Resources.cust_m1, "Marek", true),
				new SpriteEntry(Properties.Resources.cust_m2, "Piotr", true),
				new SpriteEntry(Properties.Resources.cust_m3, "Jan", true),

				new SpriteEntry(Properties.Resources.cust_f1, "Ola", false),
				new SpriteEntry(Properties.Resources.cust_f2, "Kasia", false),
				new SpriteEntry(Properties.Resources.cust_f3, "Anna", false),
			};
		}

		/// <summary>
		/// Przypisuje aktualnym klientom w kolejce losowe sprity oraz imiona do wyświetlania w UI.
		/// </summary>
		private void AssignSpritesToTodayCustomers()
		{
			_customerSprites.Clear();
			_customerDisplayNames.Clear();

			foreach (var c in _game.CustomersToday)
			{
				var entry = _spriteEntries[_rng.Next(_spriteEntries.Length)];
				_customerSprites[c] = entry.Img;
				_customerDisplayNames[c] = entry.Name;
			}
		}

		/// <summary>
		/// Przelicza docelowe pozycje sprite'ów w panelu kolejki na podstawie aktualnej kolejności klientów.
		/// </summary>
		private void RefreshQueueTargets()
		{
			int count = Math.Min(MaxVisible, _game.CustomersToday.Count);
			if (count <= 0)
			{
				_queueVisuals.Clear();
				pnlQueueView.Invalidate();
				return;
			}

			float rightEdgeX = pnlQueueView.Width - _rightMargin - SpriteW;

			List<QueueVisual> newOrder = new List<QueueVisual>();

			for (int i = 0; i < count; i++)
			{
				var customer = _game.CustomersToday[i];

				if (!_customerSprites.ContainsKey(customer) || !_customerDisplayNames.ContainsKey(customer))
				{
					var entry = _spriteEntries[_rng.Next(_spriteEntries.Length)];
					_customerSprites[customer] = entry.Img;
					_customerDisplayNames[customer] = entry.Name;
				}

				float targetX = rightEdgeX - i * _spacing;

				QueueVisual existing = _queueVisuals.FirstOrDefault(v => ReferenceEquals(v.Customer, customer));
				if (existing != null)
				{
					existing.TargetX = targetX;
					existing.Img = _customerSprites[customer];
					newOrder.Add(existing);
				}
				else
				{
					float spawnX = targetX - 140;
					newOrder.Add(new QueueVisual(customer, _customerSprites[customer], spawnX, targetX));
				}
			}

			_queueVisuals.Clear();
			_queueVisuals.AddRange(newOrder);

			pnlQueueView.Invalidate();
		}

		/// <summary>
		/// Tick animacji – interpoluje pozycje sprite'ów w kierunku TargetX.
		/// </summary>
		private void AnimTimer_Tick(object sender, EventArgs e)
		{
			if (_queueVisuals.Count == 0) return;

			bool anyMoving = false;

			for (int i = 0; i < _queueVisuals.Count; i++)
			{
				QueueVisual v = _queueVisuals[i];
				float dx = v.TargetX - v.X;

				if (Math.Abs(dx) > 0.5f)
				{
					v.X = v.X + dx * _lerp;
					anyMoving = true;
				}
				else
				{
					v.X = v.TargetX;
				}
			}

			if (anyMoving)
				pnlQueueView.Invalidate();
		}

		private void ResumeTimersIfNeeded()
		{
			// jeśli wracamy z pauzy (menu), a timery stoją -> wznów
			if (!_pausedByMenu) return;

			_pausedByMenu = false;
			_lastPatienceTickUtc = DateTime.UtcNow;

			try
			{
				if (_animTimer != null && !_animTimer.Enabled) _animTimer.Start();
			}
			catch { }

			try
			{
				if (_patienceTimer != null && !_patienceTimer.Enabled) _patienceTimer.Start();
			}
			catch { }
		}

		private void pnlQueueView_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

			if (_queueBg != null)
				e.Graphics.DrawImage(_queueBg, 0, 0, pnlQueueView.Width, pnlQueueView.Height);

			int y = pnlQueueView.Height - SpriteH + _floorYOffset;

			for (int i = _queueVisuals.Count - 1; i >= 0; i--)
			{
				QueueVisual v = _queueVisuals[i];
				int x = (int)Math.Round(v.X);
				e.Graphics.DrawImage(v.Img, x, y, SpriteW, SpriteH);
			}
		}

		private void pnlQueueView_MouseClick(object sender, MouseEventArgs e)
		{
			int y = pnlQueueView.Height - SpriteH + _floorYOffset;

			for (int i = 0; i < _queueVisuals.Count; i++)
			{
				QueueVisual v = _queueVisuals[i];
				int x = (int)Math.Round(v.X);
				Rectangle rect = new Rectangle(x, y, SpriteW, SpriteH);

				if (rect.Contains(e.Location))
				{
					if (i < dgvCustomers.Rows.Count)
					{
						dgvCustomers.ClearSelection();
						dgvCustomers.Rows[i].Selected = true;
						dgvCustomers.CurrentCell = dgvCustomers.Rows[i].Cells[0];

						_desiredCustomerIndex = i;
					}
					break;
				}
			}
		}

		private void ApplyPixelHudTheme()
		{
			this.DoubleBuffered = true;

			var hudFont = new Font("Consolas", 16, FontStyle.Bold);
			var hudFontSmall = new Font("Consolas", 10, FontStyle.Bold);
			var gridFont = new Font("Consolas", 10, FontStyle.Regular);
			var gridHeaderFont = new Font("Consolas", 10, FontStyle.Bold);

			SetupHudLabel(lblDay, hudFont);
			SetupHudLabel(lblCash, hudFont);
			SetupHudLabel(lblReputation, hudFont);

			txtLog.BorderStyle = BorderStyle.None;
			txtLog.BackColor = Color.FromArgb(0, 0, 0);
			txtLog.ForeColor = Color.Gainsboro;
			txtLog.Font = new Font("Consolas", 9, FontStyle.Regular);

			SetupHudButton(btnBuyBread, hudFontSmall);
			SetupHudButton(btnBuyMeat, hudFontSmall);
			SetupHudButton(btnBuyVeggies, hudFontSmall);
			SetupHudButton(btnBuySauce, hudFontSmall);

			SetupHudButton(btnServeSelected, hudFontSmall, accent: true);
			SetupHudButton(btnEndDay, hudFontSmall, danger: true);

			SetupHudGrid(dgvCustomers, gridFont, gridHeaderFont);
			SetupHudGrid(dgvInventory, gridFont, gridHeaderFont);

			dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvCustomers.MultiSelect = false;
			dgvCustomers.RowHeadersVisible = false;

			dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dgvInventory.MultiSelect = false;
			dgvInventory.RowHeadersVisible = false;

			MakePanelBlend(pnlLog);
			MakePanelBlend(pnlCustomers);
			MakePanelBlend(pnlInventory);
			MakePanelBlend(pnlIngredients);
			MakePanelBlend(pnlStats);
			MakePanelBlend(pnlControls);
			MakePanelBlend(pnlQueueView);
		}

		private void SetupHudLabel(Label lbl, Font font)
		{
			lbl.Font = font;
			lbl.ForeColor = Color.Gainsboro;
			lbl.BackColor = Color.Transparent;
			lbl.BorderStyle = BorderStyle.None;
		}

		private void SetupHudButton(Button b, Font font, bool accent = false, bool danger = false)
		{
			b.Font = font;

			b.FlatStyle = FlatStyle.Flat;
			b.UseVisualStyleBackColor = false;
			b.TabStop = false;
			b.Cursor = Cursors.Hand;

			b.BackColor = Color.FromArgb(20, 20, 20);
			b.ForeColor = Color.Gainsboro;

			b.FlatAppearance.BorderSize = 1;
			b.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 90);
			b.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 35, 35);
			b.FlatAppearance.MouseDownBackColor = Color.FromArgb(55, 55, 55);

			if (accent)
			{
				b.BackColor = Color.FromArgb(18, 28, 18);
				b.FlatAppearance.BorderColor = Color.FromArgb(110, 170, 110);
				b.ForeColor = Color.White;
			}

			if (danger)
			{
				b.BackColor = Color.FromArgb(28, 14, 14);
				b.FlatAppearance.BorderColor = Color.FromArgb(200, 90, 90);
				b.ForeColor = Color.White;
			}
		}

		private void SetupHudGrid(DataGridView dgv, Font cellFont, Font headerFont)
		{
			dgv.EnableHeadersVisualStyles = false;

			dgv.BorderStyle = BorderStyle.None;
			dgv.BackgroundColor = Color.FromArgb(0, 0, 0);
			dgv.GridColor = Color.FromArgb(60, 60, 60);

			dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
			dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

			dgv.ReadOnly = true;
			dgv.AllowUserToAddRows = false;
			dgv.AllowUserToDeleteRows = false;
			dgv.AllowUserToResizeRows = false;

			dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

			dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
			dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gainsboro;
			dgv.ColumnHeadersDefaultCellStyle.Font = headerFont;
			dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

			dgv.DefaultCellStyle.BackColor = Color.FromArgb(0, 0, 0);
			dgv.DefaultCellStyle.ForeColor = Color.Gainsboro;
			dgv.DefaultCellStyle.Font = cellFont;

			dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 70);
			dgv.DefaultCellStyle.SelectionForeColor = Color.White;

			dgv.RowTemplate.Height = 22;
		}

		private void MakePanelBlend(Panel p)
		{
			p.BackColor = Color.Transparent;
		}

		private static void EnableDoubleBuffering(Control control)
		{
			if (control == null) return;

			try
			{
				PropertyInfo pi = typeof(Control).GetProperty("DoubleBuffered",
					BindingFlags.Instance | BindingFlags.NonPublic);

				if (pi != null)
					pi.SetValue(control, true, null);
			}
			catch { }
		}

		// Eventy – mogą zostać puste (Designer je ma podpięte)
		private void pnlLog_Paint(object sender, PaintEventArgs e) { }
		private void dgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
		private void panel1_Paint(object sender, PaintEventArgs e) { }
		private void pnlControls_Paint(object sender, PaintEventArgs e) { }
		private void MainForm_Load(object sender, EventArgs e) { }
	}
}
