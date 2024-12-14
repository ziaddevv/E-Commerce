using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models
{
     public enum Type
	{
		Other,T_shirt, Pants,shoe,Item,Bag,
	}
	[BindProperties]
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Required]
		[Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10,000")]
		public decimal Price { get; set; }

		[Required]
		[Range(0, 1000, ErrorMessage = "Stock must be between 0 and 1000 units")]
		public int Stock { get; set; }

		[StringLength(255)]
		[Required]
		public string Description { get; set; }
		[Required]

		public Type Type { get; set; }
		public string Image { get; set; }
	 
	}

}
