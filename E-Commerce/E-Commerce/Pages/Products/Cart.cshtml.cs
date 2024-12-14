using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Commerce.Models;

namespace E_Commerce.Pages.Products
{
	public class CartModel : PageModel
	{
		private readonly CartService _cartService;

		public CartModel(CartService cartService)
		{
			_cartService = cartService;
		}

		public List<CartItem> CartItems { get; set; }

		public void OnGet()
		{
			try
			{
				CartItems = _cartService.GetCart();
			}
			catch (InvalidOperationException ex)
			{
				// Handle the case where the user is not logged in
				CartItems = new List<CartItem>();
				ViewData["Error"] = "Please log in to view your cart.";
			}
		}
	}
}
