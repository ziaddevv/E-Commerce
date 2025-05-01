using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce.Pages
{
	public class ContactModel : PageModel
	{
		[BindProperty]
		public string Name { get; set; }

		[BindProperty]
		public string Email { get; set; }

		[BindProperty]
		public string Subject { get; set; }

		[BindProperty]
		public string PhoneNumber { get; set; }

		[BindProperty]
		public string Message { get; set; }

		public string SuccessMessage { get; set; }

		public void OnGet()
		{
			 
			 
		}

		public IActionResult OnPost()
		{
			 
			 
			SuccessMessage = "Your message has been sent successfully!";
			Name = string.Empty;
			Email = string.Empty;
			Subject = string.Empty;
			PhoneNumber = string.Empty;
			Message = string.Empty;

			ModelState.Clear();

			return Page();
		}
	}
}
