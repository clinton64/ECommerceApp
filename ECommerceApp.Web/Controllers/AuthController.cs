using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerceApp.Web.Controllers;

public class AuthController : Controller
{
	private readonly IAuthService _authService;
	private readonly ITokenManager _tokenManager;
	public AuthController(IAuthService authService, ITokenManager tokenManager)
	{
		_authService = authService;
		_tokenManager = tokenManager;
	}
	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Login(LoginRequest request)
	{
		if (!ModelState.IsValid)
			return View(request);

		var response = await _authService.LoginAsync(request);

		if (response != null && response.IsSuccess)
		{
			var user = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
			
			await SignInUser(user);
			_tokenManager.SetToken(user.Token);
			return RedirectToAction("Index", "Home");
		}
		else
		{
			TempData["error"] = response?.Message;
			return View(request);
		}
	}

	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync();
		_tokenManager.ClearToken();
		return RedirectToAction(nameof(Login));
	}

	/*[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Register(RegisterRequest request)
	{
		if (!ModelState.IsValid)
			return View(request);

		var response = await _authService.RegisterAsync(request);
		if (response != null && response.IsSuccess)
		{
			var user = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
			return RedirectToAction(nameof(Login));
		}
		else
		{
			TempData["error"] = response?.Message;
			return View(request);
		}
	}*/

	private async Task SignInUser(LoginResponseDto model)
	{
		var handler = new JwtSecurityTokenHandler();

		var jwt = handler.ReadJwtToken(model.Token);

		var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
		identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
			jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
		identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
			jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
		identity.AddClaim(new Claim(JwtRegisteredClaimNames.UniqueName,
			jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.UniqueName).Value));


		identity.AddClaim(new Claim(ClaimTypes.Name,
			jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
		identity.AddClaim(new Claim(ClaimTypes.Role,
			jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role).Value));



		var principal = new ClaimsPrincipal(identity);
		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
	}
}
