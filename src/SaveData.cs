using System.Collections.Generic;

namespace TurekSimulator
{
	/// <summary>
	/// Struktura danych zapisu stanu gry.
	/// </summary>
	public class SaveData
	{
		/// <summary>
		/// Aktualny dzień rozgrywki.
		/// </summary>
		public int Day { get; set; }

		/// <summary>
		/// Ilość gotówki posiadanej przez gracza w momencie zapisu.
		/// </summary>
		public decimal Cash { get; set; }

		/// <summary>
		/// Aktualna reputacja gracza.
		/// </summary>
		public int Reputation { get; set; }

		/// <summary>
		/// Ilość poszczególnych składników w magazynie gracza.
		/// </summary>
		public int BreadQty { get; set; }
		public int MeatQty { get; set; }
		public int VeggiesQty { get; set; }
		public int SauceQty { get; set; }

		/// <summary>
		/// Lista klientów oczekujących w danym dniu (kolejka),
		/// </summary>
		public List<CustomerSave> CustomersToday { get; set; } = new List<CustomerSave>();
	}

	/// <summary>
	/// Zapis stanu pojedynczego klienta.
	/// </summary>
	public class CustomerSave
	{
		/// <summary>
		/// Imię klienta.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Początkowa cierpliwość klienta.
		/// </summary>
		public int Patience { get; set; }

		/// <summary>
		/// Pozostała cierpliwość klienta w momencie zapisu.
		/// </summary>
		public int PatienceLeft { get; set; }

		/// <summary>
		/// Cena klienta
		/// </summary>
		public decimal PriceToPay { get; set; }

		/// <summary>
		/// Zapis zamówienia klienta.
		/// </summary>
		public OrderSave Order { get; set; } = new OrderSave();
	}

	/// <summary>
	/// Zapis zamówienia klienta w uproszczonej formie.
	/// </summary>
	public class OrderSave
	{
		/// <summary>
		/// Opis tekstowy zamówienia.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Premia cenowa wynikająca z nietypowości zamówienia.
		/// </summary>
		public int PremiumFee { get; set; }

		/// <summary>
		/// Lista wymaganych składników wraz z ilościami.
		/// </summary>
		public List<RequiredPair> Required { get; set; } = new List<RequiredPair>();
	}

	/// <summary>
	/// Para składników używana przy zapisie zamówień.
	/// </summary>
	public class RequiredPair
	{
		/// <summary>
		/// Typ składnika.
		/// </summary>
		public IngredientType Type { get; set; }

		/// <summary>
		/// Wymagana ilość danego składnika.
		/// </summary>
		public int Amount { get; set; }
	}
}
