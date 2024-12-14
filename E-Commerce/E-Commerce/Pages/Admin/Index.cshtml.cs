using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            var email=HttpContext.Session.GetString("UserEmail");
			if (email != "admin@gmail.com")
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();

		}

        
    }
}
