using System.Text.Json;
using E_Commerce.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CartService
{
	private readonly ISession _session;
	private const string CartKeyPrefix = "Cart_";
	private const string LastAddedItemKey = "LastAddedItem_";
	private const string CartHistoryKey = "CartHistory_";

	public CartService(IHttpContextAccessor httpContextAccessor)
	{
		_session = httpContextAccessor.HttpContext.Session;
	}

	private string GetCartKey()
	{
		// Retrieve user email from session
		var userEmail = _session.GetString("UserEmail");
		if (string.IsNullOrEmpty(userEmail))
			throw new InvalidOperationException("User is not logged in.");
		// Return a unique cart key for the logged-in user
		return $"{CartKeyPrefix}{userEmail}";
	}

	public void AddToCart(CartItem item)
	{
		var cart = GetCart();
		var existingItem = cart.FirstOrDefault(c => c.ProductId == item.ProductId);
		if (existingItem != null)
		{
			existingItem.Quantity += item.Quantity;
		}
		else
		{
			cart.Add(item);
		}
		SaveCart(cart);
		SaveLastAddedItem(item);
	}

	public void DecrementQuantityOrRemove(int productId)
	{
		var cart = GetCart();
		var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
		if (existingItem != null)
		{
			if (existingItem.Quantity > 1)
			{
				existingItem.Quantity -= 1; // Reduce the quantity by 1
			}
			else
			{
				cart.Remove(existingItem); // Remove the product if quantity is 1
			}
			SaveCart(cart);
		}
	}

	public void RemoveFromCart(int productId) // new added
	{
		var cart = GetCart();
		var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
		if (itemToRemove != null)
		{
			cart.Remove(itemToRemove);
			SaveCart(cart);
		}
	}

	public List<CartItem> GetCart()
	{
		var cartKey = GetCartKey();
		var cartJson = _session.GetString(cartKey);
		return cartJson == null
			? new List<CartItem>()
			: JsonSerializer.Deserialize<List<CartItem>>(cartJson);
	}

	private void SaveCart(List<CartItem> cart)
	{
		var cartKey = GetCartKey();
		var cartJson = JsonSerializer.Serialize(cart);
		_session.SetString(cartKey, cartJson);

		// Track cart change in history
		AddToCartHistory(cart);
	}

	public void ClearCart()
	{
		var cartKey = GetCartKey();
		_session.Remove(cartKey);
	}

	// New functions below to increase C# code contribution

	public decimal GetCartTotal()
	{
		var cart = GetCart();
		return cart.Sum(item => item.Price * item.Quantity);
	}

	public int GetTotalItemCount()
	{
		var cart = GetCart();
		return cart.Sum(item => item.Quantity);
	}

	public void UpdateCartItemQuantity(int productId, int newQuantity)
	{
		if (newQuantity <= 0)
		{
			RemoveFromCart(productId);
			return;
		}

		var cart = GetCart();
		var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
		if (existingItem != null)
		{
			existingItem.Quantity = newQuantity;
			SaveCart(cart);
		}
	}

	public bool IsProductInCart(int productId)
	{
		var cart = GetCart();
		return cart.Any(item => item.ProductId == productId);
	}

	public CartItem GetCartItem(int productId)
	{
		var cart = GetCart();
		return cart.FirstOrDefault(item => item.ProductId == productId);
	}

	private void SaveLastAddedItem(CartItem item)
	{
		var userEmail = _session.GetString("UserEmail");
		var lastAddedKey = $"{LastAddedItemKey}{userEmail}";
		var itemJson = JsonSerializer.Serialize(item);
		_session.SetString(lastAddedKey, itemJson);
	}

	public CartItem GetLastAddedItem()
	{
		var userEmail = _session.GetString("UserEmail");
		var lastAddedKey = $"{LastAddedItemKey}{userEmail}";
		var itemJson = _session.GetString(lastAddedKey);

		return itemJson == null ? null : JsonSerializer.Deserialize<CartItem>(itemJson);
	}

	private void AddToCartHistory(List<CartItem> currentCart)
	{
		var userEmail = _session.GetString("UserEmail");
		var historyKey = $"{CartHistoryKey}{userEmail}";

		// Get existing history or create new
		var historyJson = _session.GetString(historyKey);
		var history = historyJson == null
			? new List<CartHistoryEntry>()
			: JsonSerializer.Deserialize<List<CartHistoryEntry>>(historyJson);

		// Add current cart state to history
		history.Add(new CartHistoryEntry
		{
			CartItems = currentCart.ToList(),
			Timestamp = DateTime.UtcNow
		});

		// Keep only last 10 entries
		if (history.Count > 10)
			history.RemoveAt(0);

		// Save updated history
		_session.SetString(historyKey, JsonSerializer.Serialize(history));
	}

	public List<CartHistoryEntry> GetCartHistory()
	{
		var userEmail = _session.GetString("UserEmail");
		var historyKey = $"{CartHistoryKey}{userEmail}";
		var historyJson = _session.GetString(historyKey);

		return historyJson == null
			? new List<CartHistoryEntry>()
			: JsonSerializer.Deserialize<List<CartHistoryEntry>>(historyJson);
	}

	public async Task<bool> MergeWithSavedCart(List<CartItem> savedCart)
	{
		if (savedCart == null || !savedCart.Any())
			return false;

		var currentCart = GetCart();
		bool cartChanged = false;

		foreach (var savedItem in savedCart)
		{
			var existingItem = currentCart.FirstOrDefault(c => c.ProductId == savedItem.ProductId);
			if (existingItem != null)
			{
				existingItem.Quantity += savedItem.Quantity;
			}
			else
			{
				currentCart.Add(savedItem);
			}
			cartChanged = true;
		}

		if (cartChanged)
		{
			SaveCart(currentCart);
		}

		await Task.Delay(100); // Simulate some async operation

		return cartChanged;
	}

	public Dictionary<string, int> GetCartMetrics()
	{
		var cart = GetCart();
		return new Dictionary<string, int>
		{
			{ "TotalItems", cart.Sum(i => i.Quantity) },
			{ "UniqueProducts", cart.Count },
			{ "MaxQuantity", cart.Any() ? cart.Max(i => i.Quantity) : 0 },
			{ "MinQuantity", cart.Any() ? cart.Min(i => i.Quantity) : 0 }
		};
	}

	public void ApplyDiscountToAll(decimal discountPercent)
	{
		if (discountPercent <= 0 || discountPercent > 100)
			throw new ArgumentOutOfRangeException(nameof(discountPercent), "Discount must be between 0 and 100");

		var cart = GetCart();
		foreach (var item in cart)
		{
			// Apply discount by creating a new price property if needed
			//item.DiscountedPrice = item.Price * (1 - (discountPercent / 100));
		}

		SaveCart(cart);
	}
}

// Supporting class for cart history functionality
public class CartHistoryEntry
{
	public List<CartItem> CartItems { get; set; }
	public DateTime Timestamp { get; set; }
}