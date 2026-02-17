using System;

namespace TurekSimulator
{
	/// <summary>
	/// Reprezentuje pojedynczy typ składnika dostępnego w grze.
	/// </summary>
	public class Ingredient
	{
		/// <summary>
		/// Typ składnika, używany w logice gry oraz magazynie.
		/// </summary>
		public IngredientType Type { get; }

		/// <summary>
		/// Nazwa składnika wyświetlana w interfejsie użytkownika.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Bazowa cena zakupu składnika (bez inflacji dziennej).
		/// </summary>
		public decimal BaseBuyPrice { get; }

		/// <summary>
		/// Tworzy nową definicję składnika.
		/// </summary>
		/// <param name="type">Typ składnika (enum).</param>
		/// <param name="name">Nazwa do wyświetlania w UI.</param>
		/// <param name="baseBuyPrice">Bazowa cena zakupu.</param>
		public Ingredient(IngredientType type, string name, decimal baseBuyPrice)
		{
			Type = type;
			Name = name;
			BaseBuyPrice = baseBuyPrice;
		}
	}
}
