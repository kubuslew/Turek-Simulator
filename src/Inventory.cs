using System.Collections.Generic;
using System.Linq;

namespace TurekSimulator
{
	/// <summary>
	/// Reprezentuje pojedynczy wpis w magazynie gracza:
	/// konkretny składnik oraz jego aktualną ilość.
	/// </summary>
	public class InventoryItem
	{
		/// <summary>
		/// Definicja składnika (typ, nazwa, cena bazowa).
		/// </summary>
		public Ingredient Ingredient { get; }

		/// <summary>
		/// Aktualna ilość danego składnika w magazynie.
		/// </summary>
		public int Quantity { get; set; }

		/// <summary>
		/// Tworzy nowy element magazynu dla danego składnika.
		/// </summary>
		public InventoryItem(Ingredient ingredient, int quantity)
		{
			Ingredient = ingredient;
			Quantity = quantity;
		}
	}

	/// <summary>
	/// Magazyn gracza przechowujący wszystkie dostępne składniki
	/// oraz umożliwiający ich dodawanie i zużywanie podczas obsługi klientów.
	/// </summary>
	public class Inventory
	{
		/// <summary>
		/// Wewnętrzna struktura danych magazynu:
		/// mapowanie typu składnika na odpowiadający mu obiekt InventoryItem.
		/// </summary>
		private readonly Dictionary<IngredientType, InventoryItem> _items;

		/// <summary>
		/// Lista elementów magazynu w formie tylko do odczytu,
		/// wykorzystywana głównie do prezentacji w UI.
		/// </summary>
		public IReadOnlyList<InventoryItem> Items => _items.Values.ToList();

		/// <summary>
		/// Inicjalizuje magazyn na podstawie listy dostępnych składników.
		/// Wszystkie ilości początkowo ustawione są na 0.
		/// </summary>
		public Inventory(IEnumerable<Ingredient> ingredients)
		{
			_items = ingredients.ToDictionary(i => i.Type, i => new InventoryItem(i, 0));
		}

		/// <summary>
		/// Zwraca aktualną ilość danego składnika w magazynie.
		/// </summary>
		public int GetQty(IngredientType type) => _items[type].Quantity;

		/// <summary>
		/// Dodaje określoną ilość składnika do magazynu.
		/// Używane przy zakupach w sklepie.
		/// </summary>
		public void Add(IngredientType type, int amount)
		{
			_items[type].Quantity += amount;
		}

		/// <summary>
		/// Sprawdza, czy magazyn posiada wystarczającą ilość składników
		/// do realizacji zamówienia klienta.
		/// </summary>
		public bool CanConsume(Dictionary<IngredientType, int> req)
		{
			foreach (var kv in req)
			{
				if (_items[kv.Key].Quantity < kv.Value) return false;
			}
			return true;
		}

		/// <summary>
		/// Zużywa składniki z magazynu zgodnie z wymaganiami zamówienia.
		/// Ilości nie mogą spaść poniżej zera.
		/// </summary>
		public void Consume(Dictionary<IngredientType, int> req)
		{
			foreach (var kv in req)
			{
				_items[kv.Key].Quantity -= kv.Value;
				if (_items[kv.Key].Quantity < 0) _items[kv.Key].Quantity = 0;
			}
		}

		/// <summary>
		/// jeśli składnik nie istnieje w magazynie zwraca 0.
		/// </summary>
		public int GetQuantity(IngredientType type)
		{
			return _items.TryGetValue(type, out var item) ? item.Quantity : 0;
		}

		/// <summary>
		/// Ustawia ilość danego składnika (np. podczas wczytywania zapisu gry).
		/// Wartości ujemne są automatycznie korygowane do 0.
		/// </summary>
		public void SetQuantity(IngredientType type, int qty)
		{
			if (_items.TryGetValue(type, out var item))
			{
				item.Quantity = qty < 0 ? 0 : qty;
			}
		}
	}
}
