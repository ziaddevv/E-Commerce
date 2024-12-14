using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce.Pages.Account
{
    [BindProperties]
    public class LoginModel : PageModel



    {

        public string Email { get; set; }
        public string Password { get; set; }

        private readonly ApplicationDbContext _Context;

        public LoginModel(ApplicationDbContext context)
        {
            _Context = context;
        }


        public IActionResult OnPost()
        {
            var user = _Context.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);



            if (user != null)
            {
				HttpContext.Session.SetString("UserEmail", user.Email);
				if (user.Email == "admin@gmail.com")
                {
                    return RedirectToPage("/Admin/Index");
                }
                else
                {
                    return RedirectToPage("/Products/Content");

                }

            }
            ModelState.AddModelError("", "Invalid Login Attemp.");
            return Page();
        }
        public void OnGet()
        {
        }
    }
}
