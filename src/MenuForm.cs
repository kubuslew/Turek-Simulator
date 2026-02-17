using System;
using System.Drawing;
using System.Windows.Forms;

namespace TurekSimulator
{
	public partial class MenuForm : Form
	{
		private const int DimAlpha = 160;

		private MainForm _runningGameForm;
		private GameManager _runningGameManager;
		private bool _pauseMode;

		public MenuForm()
		{
			InitializeComponent();

			DoubleBuffered = true;
			KeyPreview = true;
			this.KeyDown += MenuForm_KeyDown;

			AudioManager.StartAmbientLoop();

			ApplyMenuTheme();
			ShowAsStartMenu();
		}

		private void MenuForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape && _pauseMode)
			{
				ResumeGame();
				e.Handled = true;
			}
		}

		public void ShowAsStartMenu()
		{
			_pauseMode = false;
			_runningGameForm = null;
			_runningGameManager = null;

			btnResume.Visible = false;

			btnNewGame.Text = "NOWA GRA";
			UpdateLoadButton();
			btnExit.Text = "WYJDŹ";

			this.Show();
			this.BringToFront();
			this.Activate();
		}

		public void ShowAsPause(MainForm gameForm, GameManager gm)
		{
			_pauseMode = true;
			_runningGameForm = gameForm;
			_runningGameManager = gm;

			btnResume.Visible = true;

			btnNewGame.Text = "NOWA GRA";
			UpdateLoadButton();
			btnExit.Text = "ZAPISZ I WYJDŹ";

			this.Show();
			this.BringToFront();
			this.Activate();
		}

		private void MenuForm_Load(object sender, EventArgs e) { }

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);

			using (var brush = new SolidBrush(Color.FromArgb(DimAlpha, 0, 0, 0)))
			{
				e.Graphics.FillRectangle(brush, this.ClientRectangle);
			}
		}

		private void ApplyMenuTheme()
		{
			pnlMenu.BackColor = Color.FromArgb(230, 12, 12, 12);

			StyleButton(btnResume, accent: true);
			StyleButton(btnNewGame);
			StyleButton(btnLoadGame);
			StyleButton(btnExit, danger: true);
		}

		private void StyleButton(Button b, bool accent = false, bool danger = false)
		{
			b.FlatStyle = FlatStyle.Flat;
			b.FlatAppearance.BorderSize = 1;
			b.FlatAppearance.BorderColor = Color.FromArgb(120, 120, 120);
			b.BackColor = Color.FromArgb(20, 20, 20);
			b.ForeColor = Color.Gainsboro;
			b.Font = new Font("Consolas", 16, FontStyle.Bold);
			b.Cursor = Cursors.Hand;

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

		private void UpdateLoadButton()
		{
			if (!SaveSystem.HasSave())
			{
				btnLoadGame.Enabled = false;
				btnLoadGame.Text = "WCZYTAJ GRĘ (BRAK ZAPISU)";
				return;
			}

			var save = SaveSystem.Load();
			btnLoadGame.Enabled = save != null;
			btnLoadGame.Text = save == null ? "WCZYTAJ GRĘ (BRAK ZAPISU)" : $"WCZYTAJ GRĘ (DAY: {save.Day})";
		}

		private void btnResume_Click(object sender, EventArgs e)
		{
			ResumeGame();
		}

		private void ResumeGame()
		{
			if (_runningGameForm == null)
				return;

			this.Hide();
			_runningGameForm.Show();
			_runningGameForm.Activate();
			_runningGameForm.Focus();
		}

		private void btnNewGame_Click(object sender, EventArgs e)
		{
			if (_pauseMode && _runningGameForm != null)
			{
				_runningGameForm.Close();
				_runningGameForm = null;
				_runningGameManager = null;
			}

			var gm = new GameManager();
			OpenGame(gm);
		}

		private void btnLoadGame_Click(object sender, EventArgs e)
		{
			var save = SaveSystem.Load();
			if (save == null)
			{
				UpdateLoadButton();
				return;
			}

			if (_pauseMode && _runningGameForm != null)
			{
				_runningGameForm.Close();
				_runningGameForm = null;
				_runningGameManager = null;
			}

			var gm = new GameManager();
			gm.ApplySave(save);
			OpenGame(gm);
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			if (_pauseMode && _runningGameManager != null)
			{
				SaveSystem.Save(_runningGameManager);
			}

			Application.Exit();
		}

		private void OpenGame(GameManager gm)
		{
			this.Hide();

			var gameForm = new MainForm(gm, this);

			gameForm.FormClosed += (s, args) =>
			{
				ShowAsStartMenu();
			};

			_runningGameForm = gameForm;
			_runningGameManager = gm;

			gameForm.Show();
		}
	}
}
