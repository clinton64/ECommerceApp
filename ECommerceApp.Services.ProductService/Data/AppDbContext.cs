using ECommerceApp.Services.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.ProductService.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
	public DbSet<Product> Products { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Product>().HasData(
			new Product
			{
				Id = 1,
				Name = "IPhone 13",
				Description = "IPhone 13 from Apple",
				Category = "Smart Phone",
				Price = 999.99,
				ImageUrl = "https://fdn2.gsmarena.com/vv/bigpic/apple-iphone-15.jpg"
			},
			new Product
			{
				Id = 2,
				Name = "Samsung Galaxy S21",
				Description = "Samsung Galaxy S21 from Samsung",
				Category = "Smart Phone",
				Price = 899.99,
				ImageUrl = "https://fdn2.gsmarena.com/vv/bigpic/samsung-galaxy-s21-5g-r.jpg"
			},
			new Product
			{
				Id = 3,
				Name = "Google Pixel 6",
				Description = "Google Pixel 6 from Google",
				Category = "Smart Phone",
				Price = 799.99,
				ImageUrl = "https://fdn2.gsmarena.com/vv/pics/google/google-pixel-6-1.jpg"
			}
		);
	}
}
