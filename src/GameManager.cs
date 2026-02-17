using System;
using System.Collections.Generic;
using System.Linq;

namespace TurekSimulator
{
	/// <summary>
	/// Główny silnik logiki gry (bez zależności od UI).
	/// Odpowiada za: stan dnia, klientów, ceny, eventy, reputację oraz warunek przegranej.
	/// </summary>
	public class GameManager
	{
		/// <summary>Aktualny dzień gry (zwiększany po zakończeniu dnia).</summary>
		public int Day { get; private set; } = 1;

		/// <summary>Stan gracza: gotówka, reputacja, ekwipunek.</summary>
		public Player Player { get; private set; }

		/// <summary>Lista klientów dostępnych w bieżącym dniu.</summary>
		public List<Customer> CustomersToday { get; set; } = new List<Customer>();

		private readonly Random _rng = new Random();

		// ===== Balans ekonomii =====
		private const decimal BuyInflationPerDay = 0.05m;
		private const decimal SellInflationPerDay = 0.02m;

		// ===== Balans reputacji =====
		private const int RepServeGain = 1;
		private const int RepUnservedPenalty = 2;

		// ===== Eventy =====
		private const double AfterServeEventChance = 0.08;
		private const int DailyEventAlways = 1;

		/// <summary>
		/// Składnikowe ceny sprzedaży używane do wyliczania ceny zamówienia klienta.
		/// </summary>
		private readonly Dictionary<IngredientType, decimal> _sellComponentPrice = new Dictionary<IngredientType, decimal>
		{
			[IngredientType.Bread] = 2m,
			[IngredientType.Meat] = 6m,
			[IngredientType.Veggies] = 2m,
			[IngredientType.Sauce] = 1m
		};

		/// <summary>
		/// Lista składników dostępnych w sklepie wraz z cenami bazowymi.
		/// </summary>
		private readonly List<Ingredient> _ingredients = new List<Ingredient>
		{
			new Ingredient(IngredientType.Bread, "Buła", 2m),
			new Ingredient(IngredientType.Meat, "Mięso", 5m),
			new Ingredient(IngredientType.Sauce, "Sosiwo", 1m),
			new Ingredient(IngredientType.Veggies, "Warzywa", 2m),
		};

		/// <summary>
		/// Konstruktor nowej gry – tworzy gracza i generuje kolejkę klientów na start.
		/// </summary>
		public GameManager()
		{
			Player = new Player(startCash: 120m, startReputation: 0, inventory: new Inventory(_ingredients));
			GenerateCustomersForToday();
		}

		/// <summary>
		/// Utrata cierpliwości na sekundę (skalowanie trudności z dniem gry).
		/// </summary>
		public int PatienceLossPerSecond => 1 + (Day - 1) / 12; //+ (Day - 1) / 6;

		/// <summary>
		/// Bufor ułamkowej utraty cierpliwości (zapewnia płynność i stabilność obliczeń time-based).
		/// </summary>
		private double _patienceCarry = 0.0;

		private int _customersAtStartOfDay = 0;

		// ===== Warunek przegranej =====
		private const int LoseReputationThreshold = -50;
		private const int LoseDaysLimit = 2;                 // ile dni z rzędu poniżej progu kończy grę
		private int _daysAtOrBelowLoseThreshold = 0;

		/// <summary>Flaga informująca, czy gra została przegrana.</summary>
		public bool IsGameOver { get; private set; } = false;

		/// <summary>
		/// Aktualizuje warunek przegranej po zakończeniu dnia:
		/// jeśli reputacja jest poniżej progu przez określoną liczbę dni z rzędu, ustawia IsGameOver.
		/// </summary>
		private void UpdateGameOverAfterDayEnd()
		{
			if (Player.Reputation <= LoseReputationThreshold)
				_daysAtOrBelowLoseThreshold++;
			else
				_daysAtOrBelowLoseThreshold = 0;

			if (_daysAtOrBelowLoseThreshold >= LoseDaysLimit)
				IsGameOver = true;
		}

		/// <summary>
		/// Time-based aktualizacja cierpliwości klientów oraz zwrot listy tych, którzy odeszli.
		/// elapsedSeconds powinno pochodzić z warstwy UI (timer) jako realny czas od ostatniego ticka.
		/// </summary>
		public List<Customer> TickPatienceAndGetLeavers(double elapsedSeconds)
		{
			var leavers = new List<Customer>();
			if (elapsedSeconds <= 0) return leavers;

			double totalLoss = elapsedSeconds * PatienceLossPerSecond + _patienceCarry;
			int lossInt = (int)Math.Floor(totalLoss);
			_patienceCarry = totalLoss - lossInt;

			if (lossInt <= 0) return leavers;

			foreach (var c in CustomersToday.ToList())
			{
				if (c.IsServed) continue;

				c.PatienceLeft -= lossInt;

				if (c.PatienceLeft <= 0)
				{
					leavers.Add(c);
					CustomersToday.Remove(c);
					Player.Reputation -= 2;
				}
			}

			return leavers;
		}

		/// <summary>
		/// Zwraca cenę zakupu składnika (z inflacją zależną od dnia).
		/// </summary>
		public decimal GetBuyPrice(IngredientType type)
		{
			var ing = _ingredients.First(i => i.Type == type);
			decimal factor = 1m + BuyInflationPerDay * (Day - 1);
			return Round2(ing.BaseBuyPrice * factor);
		}

		/// <summary>
		/// Mnożnik inflacji cen sprzedaży zależny od dnia gry.
		/// </summary>
		private decimal GetSellInflationFactor()
		{
			return 1m + SellInflationPerDay * (Day - 1);
		}

		/// <summary>
		/// Mnożnik ceny zależny od reputacji gracza.
		/// </summary>
		private decimal GetReputationPriceFactor()
		{
			return ClampDecimal(1m + (Player.Reputation / 200m), 0.90m, 1.30m);
		}

		/// <summary>
		/// Wylicza cenę bazową zamówienia: suma składników + premium + inflacja + rng.
		/// </summary>
		private decimal CalculateOrderBasePrice(Order order)
		{
			decimal sum = 0m;

			foreach (var kv in order.Required)
				sum += _sellComponentPrice[kv.Key] * kv.Value;

			sum += order.PremiumFee;
			sum *= GetSellInflationFactor();

			decimal jitter = (decimal)(_rng.NextDouble() * 0.10 - 0.05);
			sum *= (1m + jitter);

			return Round2(sum);
		}

		/// <summary>
		/// Wylicza cenę końcową zamówienia z uwzględnieniem reputacji.
		/// </summary>
		private decimal CalculateFinalPrice(Order order)
		{
			decimal basePrice = CalculateOrderBasePrice(order);
			decimal repFactor = GetReputationPriceFactor();
			return Round2(basePrice * repFactor);
		}

		/// <summary>
		/// Wyznacza bazową liczbę klientów na dany dzień.
		/// </summary>
		private int GetBaseCustomersForDay()
		{
			int baseCount = 3 + (int)Math.Floor((Day - 1) / 2.0);
			return Math.Min(baseCount, 8);
		}

		/// <summary>
		/// Modyfikator liczby klientów wynikający z reputacji.
		/// </summary>
		private int GetCustomersModifierFromReputation()
		{
			int mod = Player.Reputation / 20;
			return Math.Max(-2, Math.Min(3, mod));
		}

		/// <summary>
		/// Generuje listę klientów na bieżący dzień (zamówienia, cierpliwość, ceny).
		/// </summary>
		private void GenerateCustomersForToday()
		{
			CustomersToday.Clear();

			int count = GetBaseCustomersForDay() + GetCustomersModifierFromReputation();
			count = Math.Max(1, Math.Min(10, count));

			for (int i = 0; i < count; i++)
			{
				var order = Order.Generate(_rng, Day);

				int patience = Math.Max(30 + _rng.Next(-5, 10), (60 - Day * 2) + _rng.Next(-3, 4));
				string name = "Klient";

				var c = new Customer(name, patience, order);
				c.PriceToPay = CalculateFinalPrice(order);

				CustomersToday.Add(c);
			}
			_customersAtStartOfDay = CustomersToday.Count;
		}

		/// <summary>
		/// Zakup składników do ekwipunku gracza. Rzuca wyjątek przy błędach (np. brak gotówki).
		/// </summary>
		public void BuyIngredient(IngredientType type, int amount)
		{
			if (amount <= 0) throw new Exception("Ilość musi być dodatnia.");

			decimal cost = GetBuyPrice(type) * amount;
			if (Player.Cash < cost)
				throw new Exception($"Niewystarczająca ilość gotówki, potrzeba: {cost:C}.");

			Player.Cash -= cost;
			Player.Inventory.Add(type, amount);
		}

		/// <summary>
		/// Próba obsłużenia klienta: walidacja, sprawdzenie składników, pobranie składników,
		/// dodanie gotówki, zmiana reputacji oraz ewentualny event po obsłudze.
		/// </summary>
		public bool TryServeCustomer(Customer customer, out string message)
		{
			if (customer == null)
			{
				message = "Klient???";
				return false;
			}

			if (!CustomersToday.Contains(customer))
			{
				message = "Klienta nie ma w kolejce.";
				return false;
			}

			if (customer.IsServed)
			{
				message = "Już obsłużono.";
				return false;
			}

			if (!Player.Inventory.CanConsume(customer.Order.Required))
			{
				message = $"Niewystarczająca ilość składników: {customer.Order.Description}.";
				Player.Reputation -= 1;
				return false;
			}

			Player.Inventory.Consume(customer.Order.Required);
			Player.Cash += customer.PriceToPay;

			customer.IsServed = true;
			Player.Reputation += RepServeGain;

			message = $"Sprzedano ({customer.Order.Description}) za {customer.PriceToPay:C}. Rep +{RepServeGain}.";

			if (_rng.NextDouble() < AfterServeEventChance)
			{
				var ev = GenerateAfterServeEvent();
				ApplyEvent(ev);
				message += $" | Event: {ev.Title} ({ev.CashDelta:+#;-#;0} gotówka, {ev.ReputationDelta:+#;-#;0} rep)";
			}

			return true;
		}

		/// <summary>
		/// Losowe wydarzenie „dzienne” wywoływane przyciskiem / mechaniką.
		/// </summary>
		public RandomEvent TryTriggerEvent()
		{
			if (_rng.NextDouble() < 0.10) return null;

			var ev = GenerateDailyEvent();
			ApplyEvent(ev);
			return ev;
		}

		/// <summary>
		/// Generuje event po obsłużeniu klienta (np. napiwek / skarga).
		/// </summary>
		private RandomEvent GenerateAfterServeEvent()
		{
			int roll = _rng.Next(100);

			if (roll < 55)
				return new RandomEvent("Napiwek", "Klient zostawił napiwek.", cashDelta: _rng.Next(2, 8), reputationDelta: 0);

			return new RandomEvent("Skarga", "Klient narzeka na czas obsługi.", cashDelta: 0, reputationDelta: -1);
		}

		/// <summary>
		/// Generuje event dzienny zależny od reputacji.
		/// </summary>
		private RandomEvent GenerateDailyEvent()
		{
			int rep = Player.Reputation;
			int bias = Math.Max(-10, Math.Min(10, rep / 10));
			int roll = _rng.Next(100) - bias;

			if (roll < 60)
			{
				int type = _rng.Next(3);
				if (type == 0) return new RandomEvent("Mały wydatek", "Kupiłeś środki czyszczące.", cashDelta: -_rng.Next(5, 16), reputationDelta: 0);
				if (type == 1) return new RandomEvent("Ale mamy ładny dzień", "W starej kurtce znalazłeś drobne.", cashDelta: +_rng.Next(5, 16), reputationDelta: 0);
				return new RandomEvent("Recenzja online", "Recenzja wpłynęła na Twoją reputację.", cashDelta: 0, reputationDelta: _rng.Next(-1, 2));
			}

			if (roll < 90)
			{
				int type = _rng.Next(3);
				if (type == 0) return new RandomEvent("Bonus od dostawcy", "Dostałeś kupon zniżkowy.", cashDelta: +_rng.Next(10, 26), reputationDelta: 0);
				if (type == 1) return new RandomEvent("Lokalny influencer", "Książulowi smakuje twój kebab.", cashDelta: 0, reputationDelta: +2);
				return new RandomEvent("Darmowa dostawa", "Dostawcy coś się chyba pomyliło.", cashDelta: +_rng.Next(5, 16), reputationDelta: +1);
			}

			{
				int type = _rng.Next(2);
				if (type == 0) return new RandomEvent("Inspekcja", "Nie przeszedłeś testu białej rękawiczki.", cashDelta: -_rng.Next(15, 41), reputationDelta: -1);
				return new RandomEvent("Problem z lodówką", "Część składników się zepsuła przez noc.", cashDelta: 0, reputationDelta: -1);
			}
		}

		/// <summary>
		/// Aplikuje skutki eventu do stanu gracza.
		/// </summary>
		private void ApplyEvent(RandomEvent ev)
		{
			if (ev == null) return;
			Player.Cash += ev.CashDelta;
			Player.Reputation += ev.ReputationDelta;
		}

		/// <summary>
		/// Odtwarza stan gry z obiektu SaveData (dzień, gracz, ekwipunek i kolejka klientów).
		/// </summary>
		public void ApplySave(SaveData save)
		{
			if (save == null) return;

			Day = save.Day;
			Player.Cash = save.Cash;
			Player.Reputation = save.Reputation;

			Player.Inventory.SetQuantity(IngredientType.Bread, save.BreadQty);
			Player.Inventory.SetQuantity(IngredientType.Meat, save.MeatQty);
			Player.Inventory.SetQuantity(IngredientType.Veggies, save.VeggiesQty);
			Player.Inventory.SetQuantity(IngredientType.Sauce, save.SauceQty);

			CustomersToday.Clear();

			if (save.CustomersToday != null)
			{
				foreach (var cs in save.CustomersToday)
				{
					var order = BuildOrderFromSave(cs.Order);

					var c = new Customer(cs.Name ?? "Klient", cs.Patience, order)
					{
						PriceToPay = cs.PriceToPay,
						IsServed = false
					};

					c.PatienceLeft = (cs.PatienceLeft > 0) ? cs.PatienceLeft : cs.Patience;

					CustomersToday.Add(c);
				}
			}
		}

		/// <summary>
		/// Buduje obiekt Order na podstawie danych zapisanych w SaveData.
		/// </summary>
		private static Order BuildOrderFromSave(OrderSave os)
		{
			if (os == null)
			{
				var req = new Dictionary<IngredientType, int> { [IngredientType.Bread] = 1, [IngredientType.Meat] = 1 };
				return new Order(req, "Fallback", 0);
			}

			var reqDict = new Dictionary<IngredientType, int>();

			if (os.Required != null)
			{
				foreach (var p in os.Required)
				{
					if (p.Amount <= 0) continue;
					reqDict[p.Type] = p.Amount;
				}
			}

			if (reqDict.Count == 0)
			{
				reqDict[IngredientType.Bread] = 1;
				reqDict[IngredientType.Meat] = 1;
			}

			string desc = string.IsNullOrWhiteSpace(os.Description) ? "Zamówienie" : os.Description;
			int premium = os.PremiumFee;

			return new Order(reqDict, desc, premium);
		}

		/// <summary>
		/// Kończy dzień gry: nalicza kary za nieobsłużonych, generuje event dnia,
		/// buduje raport tekstowy, sprawdza przegraną i (jeśli brak przegranej) przechodzi do kolejnego dnia.
		/// </summary>
		public string EndDayAndGetReport()
		{
			int unserved = CustomersToday.Count(c => !c.IsServed);
			if (unserved > 0)
			{
				int repLoss = unserved * RepUnservedPenalty;
				Player.Reputation -= repLoss;
			}

			RandomEvent dailyEvent = null;
			for (int i = 0; i < DailyEventAlways; i++)
			{
				dailyEvent = GenerateDailyEvent();
				ApplyEvent(dailyEvent);
			}

			var lines = new List<string>();

			int served = Math.Max(0, _customersAtStartOfDay - unserved);

			lines.Add($"Podsumowanie dnia {Day}:");
			lines.Add($"Obsłużono: {served} / {_customersAtStartOfDay}");
			if (unserved > 0) lines.Add($"Kara za nieobsłużonych: -{unserved * RepUnservedPenalty} reputacji");

			if (dailyEvent != null)
			{
				lines.Add($"Event dnia: {dailyEvent.Title} — {dailyEvent.Description} " +
						  $"(Gotówka {dailyEvent.CashDelta:+#;-#;0}, Rep {dailyEvent.ReputationDelta:+#;-#;0})");
			}

			lines.Add($"Gotówka: {Player.Cash:C}");
			lines.Add($"Reputacja: {Player.Reputation}");

			UpdateGameOverAfterDayEnd();

			// Jeśli przegrana — nie przechodzimy do kolejnego dnia.
			if (IsGameOver)
				return string.Join(Environment.NewLine, lines);

			Day++;
			_patienceCarry = 0.0;
			GenerateCustomersForToday();

			return string.Join(Environment.NewLine, lines);
		}

		/// <summary>Zaokrąglenie wartości pieniędzy do 2 miejsc.</summary>
		private static decimal Round2(decimal v) => Math.Round(v, 2);

		/// <summary>Clamp wartości dziesiętnej do zakresu [min, max].</summary>
		private static decimal ClampDecimal(decimal v, decimal min, decimal max)
		{
			if (v < min) return min;
			if (v > max) return max;
			return v;
		}
	}
}
