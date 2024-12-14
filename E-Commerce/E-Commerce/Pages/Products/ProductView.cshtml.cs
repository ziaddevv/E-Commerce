using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce.Pages.Products
{
	public class ProductViewModel : PageModel
	{
		private readonly ApplicationDbContext _db;
		private readonly CartService _cartService;

		public ProductViewModel(ApplicationDbContext db, CartService cartService)
		{
			_db = db;
			_cartService = cartService;
		}

		public Product Product { get; set; }
		public string UserEmail { get; private set; }

		public IActionResult OnGet(int id)
		{
			// Fetch the product based on its ID
			Product = _db.Products.FirstOrDefault(p => p.Id == id);

			if (Product == null)
			{
				return NotFound();
			}

			// Retrieve the user email from the session
			UserEmail = HttpContext.Session.GetString("UserEmail");

			return Page();
		}

		public async Task<IActionResult> OnPostAddToCartAsync(int productId, string productName, decimal price)
		{
			try
			{
				var item = new CartItem
				{
					ProductId = productId,
					Name = productName,
					Price = price,
					Quantity = 1
				};

				// Add item to the cart
				_cartService.AddToCart(item);
				TempData["Message"] = "Product added to cart successfully!";
			}
			catch (InvalidOperationException ex)
			{
				// Handle not logged-in case
				if (ex.Message.Contains("User must be logged in"))
				{
					return RedirectToPage("/Account/Login", new { ReturnUrl = Request.Path });
				}

				TempData["Message"] = "An error occurred while adding the product to the cart.";
			}

			return RedirectToPage();
		}
	}
}