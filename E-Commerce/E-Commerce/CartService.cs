using System.Text.Json;
using E_Commerce.Models;
using Microsoft.AspNetCore.Http;

public class CartService
{
	private readonly ISession _session;
	private const string CartKeyPrefix = "Cart_";

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
	}

	public List<CartItem> GetCart()
	{
		var cartKey = GetCartKey();
		var cartJson = _session.GetString(cartKey);

		return cartJson == null
			? new List<CartItem>()
			: JsonSerializer.Deserialize<List<CartItem>>(cartJson);
		Console.WriteLine(cartJson);
	}

	private void SaveCart(List<CartItem> cart)
	{
		var cartKey = GetCartKey();
		var cartJson = JsonSerializer.Serialize(cart);
		_session.SetString(cartKey, cartJson);
	}

	public void ClearCart()
	{
		var cartKey = GetCartKey();
		_session.Remove(cartKey);
	}
}
