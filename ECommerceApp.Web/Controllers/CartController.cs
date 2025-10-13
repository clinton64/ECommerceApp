using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ECommerceApp.Web.Controllers;

public class CartController : Controller
{
	private readonly ICartService _cartService;

	public CartController(ICartService cartService)
	{
		_cartService = cartService;
	}
	public async Task<IActionResult> Index()
	{
		return View(await LoadCart());
	}

	public async Task<IActionResult> ApplyCoupon(CartDto cart)
	{
		var response = await _cartService.ApplyCouponAsync(cart);

		if (response != null && response.IsSuccess)
		{
			TempData["success"] = "Cart updated successfully";
			return RedirectToAction(nameof(Index));
		}
		return View();
	}

	private async Task<CartDto> LoadCart()
	{

		var userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
		var response = await _cartService.GetCartByUserIdAsnyc(userId);

		if (response != null && response.IsSuccess)
		{
			return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result)); 
		}
		return new CartDto();
	}
}
