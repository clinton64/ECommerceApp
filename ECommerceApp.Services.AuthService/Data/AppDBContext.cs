using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.AuthService.Dara;

public class AppDBContext : IdentityDbContext<IdentityUser>
{
	public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
	}
}
