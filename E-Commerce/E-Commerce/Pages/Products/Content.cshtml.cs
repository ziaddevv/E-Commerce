using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace E_Commerce.Pages.Products
{
	public class ContentModel : PageModel
	{
		private readonly ApplicationDbContext _db;

		public IEnumerable<Product> Products { get; set; }

		public string CurrentFilter { get; set; }

		public ContentModel(ApplicationDbContext db)
		{
			_db = db;
		}

		public void OnGet(string productType=null)
		{
			CurrentFilter = productType;

			Products = string.IsNullOrEmpty(productType)
				? _db.Products : _db.Products.Where(p => p.Type.ToString() == productType);
			 
		}

	 


	}
}
