using System;
using System.Collections.Generic;

namespace TurekSimulator
{
	/// <summary>
	/// Model zamówienia klienta: określa wymagane składniki, opis tekstowy oraz dopłatę (premium).
	/// Obiekt ten jest wykorzystywany przez logikę gry do weryfikacji magazynu oraz wyliczania ceny sprzedaży.
	/// </summary>
	public class Order
	{
		/// <summary>
		/// Słownik wymagań zamówienia: typ składnika -> ilość wymagana do realizacji.
		/// </summary>
		public Dictionary<IngredientType, int> Required { get; }

		/// <summary>
		/// Opis zamówienia do prezentacji w UI/logu (np. "Standard + Warzywa + Sosiwo").
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Dodatkowa opłata za bardziej wymagające zamówienia (ułatwia skalowanie opłacalności).
		/// </summary>
		public int PremiumFee { get; }

		/// <summary>
		/// Tworzy nowe zamówienie o zadanych wymaganiach, opisie oraz dopłacie premium.
		/// </summary>
		public Order(Dictionary<IngredientType, int> required, string description, int premiumFee)
		{
			Required = required;
			Description = description;
			PremiumFee = premiumFee;
		}

		/// <summary>
		/// Generator losowych zamówień. Rozkład typów zamówień jest sterowany przez RNG oraz dzień gry,
		/// co pozwala stopniowo zwiększać trudność (np. większa szansa na dodatkowe mięso wraz z dniem).
		/// </summary>
		public static Order Generate(Random rng, int day)
		{
			int roll = rng.Next(100);

			// Najczęstszy wariant: klasyczne zamówienie z opcjonalnymi dodatkami.
			if (roll < 75)
			{
				var req = new Dictionary<IngredientType, int>
				{
					[IngredientType.Bread] = 1,
					[IngredientType.Meat] = 1
				};
				bool veg = rng.NextDouble() < 0.50;
				bool sauce = rng.NextDouble() < 0.60;

				if (veg) req[IngredientType.Veggies] = 1;
				if (sauce) req[IngredientType.Sauce] = 1;

				string desc = "Standard";
				if (veg) desc += " + Warzywa";
				if (sauce) desc += " + Sosiwo";
				return new Order(req, desc, premiumFee: 0);
			}

			// Rzadszy wariant: modyfikacje zamówienia oraz dopłaty premium (skalowane dniem).
			if (roll < 95)
			{
				double extraMeatChance = Math.Min(0.20 + day * 0.03, 0.55);
				double noSauceChance = 0.18;
				double noVegChance = 0.18;

				var req = new Dictionary<IngredientType, int>
				{
					[IngredientType.Bread] = 1,
					[IngredientType.Meat] = 1
				};

				string desc = "Wariant";
				int premium = 1;

				if (rng.NextDouble() < extraMeatChance)
				{
					req[IngredientType.Meat] = 2;
					desc += " (Dod. Mięso)";
					premium += 2;
				}

				bool noSauce = rng.NextDouble() < noSauceChance;
				bool noVeg = rng.NextDouble() < noVegChance;

				if (!noVeg) req[IngredientType.Veggies] = 1; else { desc += " (Bez Warz.)"; premium += 1; }
				if (!noSauce) req[IngredientType.Sauce] = 1; else { desc += " (Bez sos.)"; premium += 1; }

				return new Order(req, desc, premiumFee: premium);
			}

			// Najrzadszy wariant: "dziwne" zamówienia specjalne (większa dopłata premium).
			{
				int weirdType = rng.Next(4);
				var req = new Dictionary<IngredientType, int>();
				string desc = "Inne: ";
				int premium = 4;

				switch (weirdType)
				{
					case 0:
						req[IngredientType.Meat] = 1;
						req[IngredientType.Veggies] = 1;
						req[IngredientType.Sauce] = 1;
						desc += "Bez bułki";
						premium += 2;
						break;

					case 1:
						req[IngredientType.Bread] = 1;
						req[IngredientType.Meat] = 3;
						req[IngredientType.Sauce] = 1;
						desc += "Kula mocy (3x mięso)";
						premium += 5;
						break;

					case 2:
						req[IngredientType.Bread] = 1;
						req[IngredientType.Meat] = 2;
						req[IngredientType.Sauce] = 1;
						desc += "Mięso + sosiwo";
						premium += 3;
						break;

					default:
						req[IngredientType.Bread] = 1;
						req[IngredientType.Veggies] = 2;
						req[IngredientType.Sauce] = 1;
						desc += "Warzywny specjał";
						premium += 3;
						break;
				}

				return new Order(req, desc, premiumFee: premium);
			}
		}
	}
}
