namespace TurekSimulator
{
	/// <summary>
	/// Reprezentuje pojedynczego klienta w kolejce.
	/// Klasa przechowuje zarówno dane logiczne (cierpliwość, zamówienie),
	/// jak i stan obsługi wykorzystywany przez GameManager oraz UI.
	/// </summary>
	public class Customer
	{
		/// <summary>
		/// Imię klienta (techniczne / bazowe).
		/// W UI może być nadpisywane losowym imieniem wyświetlanym.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Maksymalna cierpliwość klienta (wartość początkowa).
		/// Służy m.in. do wyliczania procentu cierpliwości w UI.
		/// </summary>
		public int Patience { get; set; }

		/// <summary>
		/// Aktualna pozostała cierpliwość klienta.
		/// Zmniejszana w czasie przez logikę timera w GameManager.
		/// </summary>
		public int PatienceLeft { get; set; }

		/// <summary>
		/// Zamówienie klienta – lista wymaganych składników
		/// wraz z opisem i ewentualną premią cenową.
		/// </summary>
		public Order Order { get; set; }

		/// <summary>
		/// Flaga informująca, czy klient został obsłużony.
		/// Wykorzystywana przy podsumowaniu dnia oraz logice reputacji.
		/// </summary>
		public bool IsServed { get; set; }

		/// <summary>
		/// Finalna kwota, jaką klient zapłaci po obsłużeniu.
		/// Wyliczana w GameManager na podstawie zamówienia,
		/// dnia gry oraz reputacji gracza.
		/// </summary>
		public decimal PriceToPay { get; set; }

		/// <summary>
		/// Tworzy nowego klienta z określoną cierpliwością i zamówieniem.
		/// Początkowo klient nie jest obsłużony, a jego aktualna cierpliwość
		/// równa się wartości początkowej.
		/// </summary>
		public Customer(string name, int patience, Order order)
		{
			Name = name;
			Patience = patience;
			PatienceLeft = patience;
			Order = order;
			IsServed = false;
		}
	}
}
