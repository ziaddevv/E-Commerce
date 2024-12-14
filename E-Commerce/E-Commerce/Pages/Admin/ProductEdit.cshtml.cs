using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Type = E_Commerce.Models.Type;

namespace E_Commerce.Pages.Admin
{
	public class ProductEditModel : PageModel
	{
		private readonly IProductRepository _productRepository;
		private readonly IWebHostEnvironment _environment;

		[BindProperty]
		public Product Product { get; set; }

		[BindProperty]
		public IFormFile? Photo { get; set; }

		[BindProperty]
		public List<SelectListItem> TypeList { get; set; }

		public ProductEditModel(IProductRepository productRepository, IWebHostEnvironment environment)
		{
			_productRepository = productRepository;
			_environment = environment;
			TypeList = Enum.GetValues(typeof(Type))
						   .Cast<Type>()
						   .Select(t => new SelectListItem { Text = t.ToString(), Value = t.ToString() })
						   .ToList();
		}

		public IActionResult OnGet(int id)
		{
			Product = _productRepository.GetProductById(id);
			if (Product == null)
			{
				return RedirectToPage("Index");
			}
			return Page();
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			if (Photo != null)
			{
				string imagePath = HandleImageUpload(Product.Image);
				Product.Image = imagePath;
			}

			_productRepository.UpdateProduct(Product);

			return RedirectToPage("Index");
		}

		private string HandleImageUpload(string existingImage)
		{
			if (!string.IsNullOrEmpty(existingImage))
			{
				string oldImagePath = Path.Combine(_environment.WebRootPath, existingImage);
				if (System.IO.File.Exists(oldImagePath))
				{
					System.IO.File.Delete(oldImagePath);
				}
			}

			string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
			string filePath = Path.Combine(_environment.WebRootPath, "Images", uniqueFileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				Photo.CopyTo(fileStream);
			}

			return "/Images/" + uniqueFileName;
		}
	}
}
