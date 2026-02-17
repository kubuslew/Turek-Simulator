namespace TurekSimulator
{
	/// <summary>
	/// Reprezentuje losowe zdarzenie w grze, które może wpłynąć
	/// na stan finansowy gracza oraz jego reputację.
	/// </summary>
	public class RandomEvent
	{
		/// <summary>
		/// Tytuł zdarzenia wyświetlany graczowi.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Opis zdarzenia.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Zmiana ilości gotówki gracza wynikająca ze zdarzenia
		/// </summary>
		public int CashDelta { get; }

		/// <summary>
		/// Zmiana reputacji gracza wynikająca ze zdarzenia
		/// </summary>
		public int ReputationDelta { get; }

		/// <summary>
		/// Tworzy nowe losowe zdarzenie z określonymi skutkami ekonomicznymi i reputacyjnymi.
		/// </summary>
		/// <param name="title">Krótki tytuł zdarzenia.</param>
		/// <param name="description">Opis zdarzenia.</param>
		/// <param name="cashDelta">Zmiana gotówki.</param>
		/// <param name="reputationDelta">Zmiana reputacji.</param>
		public RandomEvent(string title, string description, int cashDelta, int reputationDelta)
		{
			Title = title;
			Description = description;
			CashDelta = cashDelta;
			ReputationDelta = reputationDelta;
		}
	}
}
