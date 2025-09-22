using ECommerceApp.Services.CouponService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.CouponService.Data;

public class AppDBContext : DbContext
{
	public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
	{
		
	}

	public DbSet<Coupon> Coupons { get; set; }	

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);


		/*modelBuilder.Entity<Coupon>().HasData(
			new Coupon
			{
				Id = 1,
				CouponCode = "10OFF",
				DiscountAmount = 10,
				MinimumAmount = 50
			},
			new Coupon
			{
				Id = 2,
				CouponCode = "20OFF",
				DiscountAmount = 20,
				MinimumAmount = 100
			}
		);*/
	}
}
