using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace E_Commerce.Models
{
	[BindProperties]
	public class User
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[NotMapped]
		[Required(ErrorMessage = "Confirm Password is required")]
		[Compare("Password", ErrorMessage = "Passwords do not match")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }


		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; }
	}
}
