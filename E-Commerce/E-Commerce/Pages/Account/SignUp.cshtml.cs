using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly ApplicationDbContext _Context;

        public SignUpModel(ApplicationDbContext context)
        {
            _Context = context;
        }
        [BindProperty]
        public User User { get; set; }


       

	 
 
		public IActionResult OnPost()
        {

			if (!ModelState.IsValid)
			{
				return Page();
			} 
			if (User.Email.ToLower() == "admin@gmail.com")
            {
                ModelState.AddModelError(nameof(User.Email), "This email is reserved . Please choose another email.");
                return Page();
            }
            var existingUser = _Context.Users.FirstOrDefault(u => u.Email.ToLower() == User.Email.ToLower());
            if (existingUser != null)
            {

                ModelState.AddModelError(nameof(User.Email), "An account with this Email already exists. Please use another Email.");

                return Page();
            }


            _Context.Users.Add(User);
            _Context.SaveChanges();

            return RedirectToPage("/Account/Login");
        }
        public void OnGet()
        {
        }
    }
}
