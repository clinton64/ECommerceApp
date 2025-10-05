﻿namespace ECommerceApp.Services.ProductService.Models.DTO;

public class ProductDto
{
	public int Id { get; set; }	
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;

	public double Price { get; set; }
	public string Category { get; set; } = string.Empty;

	public string? ImageUrl { get; set; } = string.Empty;

	public string? ImageLocalPath { get; set; } = string.Empty;	

	public IFormFile? Image { get; set; }
}
