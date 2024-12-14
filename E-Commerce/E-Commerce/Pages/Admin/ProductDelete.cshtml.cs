using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce.Pages.Admin
{
	public class ProductDeleteModel : PageModel
	{
		private readonly IProductRepository _productRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public Product Product { get; set; }

		public ProductDeleteModel(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
		{
			_productRepository = productRepository;
			_webHostEnvironment = webHostEnvironment;
		}

		public void OnGet(int? id)
		{
			if (id == null)
			{
				Response.Redirect("/Admin/Index");
				return;
			}

			var product = _productRepository.GetProductById(id.Value);
			if (product == null)
			{
				Response.Redirect("/Admin/Index");
				return;
			}

			// Delete the product image from the file system
			if (!string.IsNullOrEmpty(product.Image))
			{
				string imageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, product.Image);
				if (System.IO.File.Exists(imageFullPath))
				{
					System.IO.File.Delete(imageFullPath);
				}
			}

			// Remove the product from the database
			_productRepository.DeleteProduct(id.Value);

			Response.Redirect("/Admin/Index");
		}
	}
}
