using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Type = E_Commerce.Models.Type;

namespace E_Commerce.Pages.Admin
{
	public class CreateModel : PageModel
	{
		private readonly IProductRepository _productRepository;
		private readonly IWebHostEnvironment _environment;

		[BindProperty]
		public Product Product { get; set; }

		[BindProperty]
		public IFormFile Photo { get; set; }

		public List<SelectListItem> TypeList { get; set; }

		public CreateModel(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
		{
			_productRepository = productRepository;
			_environment = webHostEnvironment;

			TypeList = Enum.GetValues(typeof(Type))
						   .Cast<Type>()
						   .Select(t => new SelectListItem { Text = t.ToString(), Value = t.ToString() })
						   .ToList();
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			if (Photo != null)
			{
				try
				{
					Product.Image = "/Images/" + ProcessUploadedFile();
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, $"File upload failed: {ex.Message}");
					return Page();
				}
			}

			_productRepository.AddProduct(Product);
			return RedirectToPage("Index");
		}

		private string ProcessUploadedFile()
		{
			string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
			string imagesFolder = Path.Combine(_environment.WebRootPath, "Images");
			string filePath = Path.Combine(imagesFolder, uniqueFileName);

			Directory.CreateDirectory(imagesFolder); // Ensure the folder exists
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				Photo.CopyTo(fileStream); // Save the file
			}

			return uniqueFileName;
		}

		public void OnGet()
		{
		}
	}
}
