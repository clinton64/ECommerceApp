using ECommerceApp.Services.AuthService.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.AuthService.Data;

public class DBInitializer : IDBInitializer
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly AppDBContext _context;

	public DBInitializer(
		UserManager<IdentityUser> userManager,
		RoleManager<IdentityRole> roleManager,
		AppDBContext context)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_context = context;
	}
	public void initialize()
	{
		// migrations if they are not applied
		try
		{
			if (_context.Database.GetPendingMigrations().Count() > 0)
			{
				_context.Database.Migrate();
			}
		}
		catch (Exception e)
		{

		}
		// create roles if they are not created yet
		if (!_roleManager.RoleExistsAsync(SD.Role_User_Admin).GetAwaiter().GetResult())
		{
			_roleManager.CreateAsync(new IdentityRole(SD.Role_User_Admin)).GetAwaiter().GetResult(); ;
			_roleManager.CreateAsync(new IdentityRole(SD.Role_User_Manager)).GetAwaiter().GetResult(); ;
			_roleManager.CreateAsync(new IdentityRole(SD.Role_User_Monitor)).GetAwaiter().GetResult(); ;
			_roleManager.CreateAsync(new IdentityRole(SD.Role_User_Customer)).GetAwaiter().GetResult(); ;

			// create admin user as well
			_userManager.CreateAsync(new IdentityUser
			{
				UserName = "Admin",
				Email = "Admin@gmail.com",
				PhoneNumber = "12345",
			}, "Hello@111").GetAwaiter().GetResult();

			var user = _context.Users.FirstOrDefault(u => u.Email == "Admin@gmail.com");
			_userManager.AddToRoleAsync(user, SD.Role_User_Admin).GetAwaiter().GetResult();
		}
	}
}
