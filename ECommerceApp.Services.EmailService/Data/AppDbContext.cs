using ECommerceApp.Services.EmailService.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.EmailService.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
	public DbSet<EmailLogger> EmailLoggers { get; set; }
}
