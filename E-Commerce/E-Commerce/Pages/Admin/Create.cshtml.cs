using E_Commerce;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
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
//private async Task LoadFormDependencies()
//{
//	// Get categories for dropdown
//	var categories = await _categoryService.GetAllCategoriesAsync();
//	CategoryList = categories.Select(c => new SelectListItem
//	{
//		Text = c.Name,
//		Value = c.Id.ToString()
//	}).ToList();

//	// Get vendors for dropdown (pseudo-code)
//	VendorList = await GetVendorListItems();
//}

//private async Task<List<SelectListItem>> GetVendorListItems()
//{
//	// Simulated vendor list - in a real app this would come from a database
//	return new List<SelectListItem>
//			{
//				new SelectListItem { Text = "Vendor 1", Value = "1" },
//				new SelectListItem { Text = "Vendor 2", Value = "2" },
//				new SelectListItem { Text = "Vendor 3", Value = "3" }
//			};
//}

//public async Task<IActionResult> OnPostAsync()
//{
//	_logger.LogInformation("Product creation attempt started");

//	if (!ModelState.IsValid)
//	{
//		_logger.LogWarning("Product validation failed: {Errors}",
//			string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
//		await LoadFormDependencies();
//		return Page();
//	}

//	// Additional validation logic
//	if (!ValidateProductData())
//	{
//		await LoadFormDependencies();
//		return Page();
//	}

//	try
//	{
//		// Process main product photo
//		if (Photo != null)
//		{
//			Product.Image = "/Images/" + await ProcessUploadedFileAsync(Photo);

//			// Generate thumbnail
//			string thumbnailPath = await GenerateThumbnailAsync(Photo);
//			Product.ThumbnailImage = "/Images/Thumbnails/" + thumbnailPath;
//		}

//		// Process additional photos if any
//		if (AdditionalPhotos != null && AdditionalPhotos.Count > 0)
//		{
//			Product.AdditionalImages = new List<string>();
//			foreach (var additionalPhoto in AdditionalPhotos.Take(5)) // Limit to 5 additional photos
//			{
//				string fileName = await ProcessUploadedFileAsync(additionalPhoto, "Additional");
//				Product.AdditionalImages.Add("/Images/Additional/" + fileName);
//			}
//		}

//		// Set creation metadata
//		Product.Created = DateTime.UtcNow;
//		Product.CreatedBy = User.FindFirst("sub")?.Value;

//		// Apply any additional metadata
//		ApplyProductMetadata();

//		// Add product to repository
//		var addedProduct = await _productRepository.AddProductAsync(Product);
//		_logger.LogInformation("Product successfully created with ID: {ProductId}", addedProduct.Id);

//		// Index the product for search (pseudo-code)
//		await IndexProductForSearch(addedProduct);

//		TempData["SuccessMessage"] = $"Product '{Product.Name}' was successfully created.";
//		return RedirectToPage("Index");
//	}
//	catch (Exception ex)
//	{
//		_logger.LogError(ex, "Error occurred while creating product");
//		ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
//		await LoadFormDependencies();
//		return Page();
//	}
//}

//private bool ValidateProductData()
//{
//	bool isValid = true;

//	// Check if price is reasonable
//	if (Product.Price <= 0)
//	{
//		ModelState.AddModelError("Product.Price", "Price must be greater than zero.");
//		isValid = false;
//	}

//	// Check if there's a price ceiling
//	if (Product.Price > 10000)
//	{
//		ModelState.AddModelError("Product.Price", "Price cannot exceed $10,000.");
//		isValid = false;
//	}

//	// Validate stock
//	if (Product.Stock < 0)
//	{
//		ModelState.AddModelError("Product.Stock", "Stock cannot be negative.");
//		isValid = false;
//	}

//	// Validate image size and format
//	if (Photo != null)
//	{
//		if (Photo.Length > 5 * 1024 * 1024) // 5MB limit
//		{
//			ModelState.AddModelError("Photo", "Image size cannot exceed 5MB.");
//			isValid = false;
//		}

//		string ext = Path.GetExtension(Photo.FileName).ToLowerInvariant();
//		if (string.IsNullOrEmpty(ext) || !new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(ext))
//		{
//			ModelState.AddModelError("Photo", "Only JPG, PNG and GIF images are allowed.");
//			isValid = false;
//		}
//	}

//	return isValid;
//}

//private void ApplyProductMetadata()
//{
//	if (Metadata != null)
//	{
//		// Apply SEO metadata
//		Product.MetaTitle = !string.IsNullOrEmpty(Metadata.MetaTitle)
//			? Metadata.MetaTitle
//			: Product.Name;

//		Product.MetaDescription = Metadata.MetaDescription;
//		Product.Keywords = Metadata.Keywords;

//		// Apply shipping metadata
//		Product.Weight = Metadata.Weight;
//		Product.Dimensions = Metadata.Dimensions;
//		Product.IsShippable = Metadata.IsShippable;

//		// Apply inventory metadata
//		Product.SKU = Metadata.SKU ?? GenerateSkuCode();
//		Product.MinimumStockLevel = Metadata.MinimumStockLevel;
//		Product.IsBackorderable = Metadata.IsBackorderable;
//	}
//}

//private string GenerateSkuCode()
//{
//	// Generate a unique SKU based on product type and timestamp
//	string typePrefix = Product.Type.ToString().Substring(0, 2).ToUpperInvariant();
//	string timestamp = DateTime.UtcNow.ToString("yyMMddHHmm");
//	return $"{typePrefix}-{timestamp}";
//}

//private string ProcessUploadedFile()
//{
//	string uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
//	string imagesFolder = Path.Combine(_environment.WebRootPath, "Images");
//	string filePath = Path.Combine(imagesFolder, uniqueFileName);
//	Directory.CreateDirectory(imagesFolder); // Ensure the folder exists
//	using (var fileStream = new FileStream(filePath, FileMode.Create))
//	{
//		Photo.CopyTo(fileStream); // Save the file
//	}
//	return uniqueFileName;
//}