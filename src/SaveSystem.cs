using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TurekSimulator
{
	/// <summary>
	/// System odpowiedzialny za zapis i odczyt stanu gry do/z pliku.
	/// </summary>
	public static class SaveSystem
	{
		/// <summary>
		/// Nazwa pliku zapisu gry.
		/// </summary>
		private const string FileName = "save.xml";

		/// <summary>
		/// Pełna ścieżka do pliku zapisu, oparta o katalog aplikacji.
		/// </summary>
		public static string SavePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);

		/// <summary>
		/// Sprawdza, czy plik zapisu istnieje.
		/// </summary>
		public static bool HasSave() => File.Exists(SavePath);

		/// <summary>
		/// Zapisuje aktualny stan gry do pliku XML.
		/// Przekształca obiekt GameManager na strukturę SaveData
		/// </summary>
		public static void Save(GameManager gm)
		{
			var save = new SaveData
			{
				Day = gm.Day,
				Cash = gm.Player.Cash,
				Reputation = gm.Player.Reputation,

				BreadQty = gm.Player.Inventory.GetQuantity(IngredientType.Bread),
				MeatQty = gm.Player.Inventory.GetQuantity(IngredientType.Meat),
				VeggiesQty = gm.Player.Inventory.GetQuantity(IngredientType.Veggies),
				SauceQty = gm.Player.Inventory.GetQuantity(IngredientType.Sauce),
			};

			// Zapis aktualnej kolejki klientów wraz z ich zamówieniami
			save.CustomersToday = gm.CustomersToday.Select(c => new CustomerSave
			{
				Name = c.Name,
				Patience = c.Patience,
				PatienceLeft = c.PatienceLeft,
				PriceToPay = c.PriceToPay,
				Order = new OrderSave
				{
					Description = c.Order?.Description ?? "Zamówienie",
					PremiumFee = c.Order?.PremiumFee ?? 0,
					Required = c.Order?.Required?.Select(kv => new RequiredPair
					{
						Type = kv.Key,
						Amount = kv.Value
					}).ToList() ?? new System.Collections.Generic.List<RequiredPair>()
				}
			}).ToList();

			var serializer = new XmlSerializer(typeof(SaveData));
			using (var fs = new FileStream(SavePath, FileMode.Create))
			{
				serializer.Serialize(fs, save);
			}
		}

		/// <summary>
		/// Wczytuje zapis gry z pliku XML i zwraca obiekt SaveData.
		/// W przypadku błędu (np. uszkodzony plik) zwraca null.
		/// </summary>
		public static SaveData Load()
		{
			if (!HasSave()) return null;

			try
			{
				var serializer = new XmlSerializer(typeof(SaveData));
				using (var fs = new FileStream(SavePath, FileMode.Open))
				{
					return (SaveData)serializer.Deserialize(fs);
				}
			}
			catch
			{
				// Błąd deserializacji – traktujemy zapis jako nieprawidłowy
				return null;
			}
		}

		/// <summary>
		/// Usuwa plik zapisu gry.
		///np. przy rozpoczęciu nowej gry.
		/// </summary>
		public static void DeleteSave()
		{
			if (HasSave())
				File.Delete(SavePath);
		}
	}
}
