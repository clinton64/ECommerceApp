using ECommerceApp.Services.OrderService.Model;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.OrderService.Data;

public class AppDbContext: DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<OrderDetail> OrderDetails { get; set; }
	public DbSet<OrderHeader> OrderHeaders { get; set; }
}
