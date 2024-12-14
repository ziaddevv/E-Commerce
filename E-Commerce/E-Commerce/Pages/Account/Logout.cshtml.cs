using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce.Pages.Account
{
	public class LogoutModel : PageModel
	{
		public IActionResult OnGet()
		{
			HttpContext.Session.Clear(); // Clear session
			return RedirectToPage("/Account/Login"); // Redirect to Login page
		}
	}
}
