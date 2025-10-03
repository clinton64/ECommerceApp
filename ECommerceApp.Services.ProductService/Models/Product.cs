using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Services.ProductService.Models;

public class Product
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;

	[Range(1,1000)]
	public double Price { get; set; }
	public string Category { get; set; } = string.Empty;

	public string ImageUrl { get; set; } = string.Empty;

	public string ImageLocalPath { get; set; } = string.Empty;
}
