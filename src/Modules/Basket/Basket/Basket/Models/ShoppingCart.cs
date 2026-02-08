namespace Basket.Basket.Models;

public class ShoppingCart : Aggregate<Guid>
{
	private readonly List<ShoppingCartItem> _items = [];

	public string UserName { get; private set; } = default!;	
	public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();
	public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

	public static ShoppingCart Create(Guid id, string userName)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(userName);
		var shoppingCart = new ShoppingCart
		{
			Id = id,
			UserName = userName
		};

		return shoppingCart;
	}

	public void AddItem(Guid productId, int quantity, string color, decimal price, string productName)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

		var existingItem = Items.FirstOrDefault(x => x.ProductId == productId);
		if (existingItem is not null)
			existingItem.Quantity += quantity;
		else
		{
			var newItem = new ShoppingCartItem(Id, productId, quantity, color, price, productName);
			_items.Add(newItem);
		}
	}

	public void RemoveItem(Guid productId)
	{
		var itemToRemove = Items.FirstOrDefault(x => x.ProductId == productId);
		if (itemToRemove is not null)
			_items.Remove(itemToRemove);
	}
}
