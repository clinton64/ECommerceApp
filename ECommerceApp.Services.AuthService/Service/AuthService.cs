using ECommerceApp.Services.AuthService.Data;
using ECommerceApp.Services.AuthService.Models.DTO;
using ECommerceApp.Services.AuthService.Service.IService;
using ECommerceApp.Services.AuthService.Utility;
using Microsoft.AspNetCore.Identity;

namespace ECommerceApp.Services.AuthService.Service;

public class AuthService : IAuthService
{
	private readonly AppDBContext _context ;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly ITokenGenerator _tokenGenerator;

	public AuthService(AppDBContext context, UserManager<IdentityUser> userManager, ITokenGenerator tokenGenerator)
	{
		_context = context;
		_userManager = userManager;	
		_tokenGenerator = tokenGenerator;
	}
	public async Task<LoginResponse> Login(LoginRequest request)
	{
		var user = _context.Users.FirstOrDefault(u => u.Email == request.UserName);
		var isValid = await _userManager.CheckPasswordAsync(user, request.Password);

		if (user == null || !isValid)
			return new LoginResponse() { User = null, Token = string.Empty };

		// get token
		var roles = await _userManager.GetRolesAsync(user);	
		var token = _tokenGenerator.GenerateToken(user, roles);

		return new LoginResponse
		{
			User = new UserDto
			{
				ID = user.Id, Email = user.Email, Name = user.UserName, PhoneNumber = user.PhoneNumber
			}, 
			Token = token
		};
	}

	public async Task<string> Register(RegistrationRequest request)
	{
		var user = new IdentityUser
		{
			UserName = request.Name,
			Email = request.Email,
			NormalizedEmail = request.Email.ToUpper(),
			PhoneNumber = request.PhoneNumber
		};

		try
		{
			var result = await _userManager.CreateAsync(user, request.Password);
			if (result.Succeeded)
			{
				var createdUser = _context.Users.First(u => u.Email == request.Email);
				await _userManager.AddToRoleAsync(createdUser, SD.Role_User_Customer);
				return string.Empty;
			}
			return result.Errors.FirstOrDefault().Description;
		}
		catch (Exception ex)
		{
		}
		return "Error encountered";
	}
}
