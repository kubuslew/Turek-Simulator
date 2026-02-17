namespace TurekSimulator
{
	/// <summary>
	/// Model gracza: przechowuje stan gotowki, reputację oraz magazyn składników.
	/// </summary>
	public class Player
	{
		/// <summary>
		/// Aktualna ilość gotówki gracza.
		/// </summary>
		public decimal Cash { get; set; }

		/// <summary>
		/// Aktualna reputacja gracza (wpływa m.in. na ceny i liczbę klientów).
		/// </summary>
		public int Reputation { get; set; }

		/// <summary>
		/// Magazyn składników gracza.
		/// </summary>
		public Inventory Inventory { get; }

		/// <summary>
		/// Tworzy gracza z zadanym stanem początkowym: gotówką, reputacją oraz referencją do magazynu.
		/// </summary>
		public Player(decimal startCash, int startReputation, Inventory inventory)
		{
			Cash = startCash;
			Reputation = startReputation;
			Inventory = inventory;
		}
	}
}
