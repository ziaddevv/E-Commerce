using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace E_Commerce.Pages.Products
{
	public class ContentModel : PageModel
	{
		private readonly IProductRepository _productRepository;

		public IEnumerable<Product> Products { get; set; } // List to store the products
		public string CurrentFilter { get; set; } // To store the current filter value

		public ContentModel(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public void OnGet(string productType = null) // This comes from the URL
		{
			CurrentFilter = productType;

			// Use the repository to fetch products based on the filter
			Products = string.IsNullOrEmpty(productType)
				? _productRepository.GetAllProducts() // Get all products
				: _productRepository.GetAllProducts().Where(p => p.Type.ToString() == productType);
		}
	}
}
