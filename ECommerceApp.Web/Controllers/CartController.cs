using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using ECommerceApp.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace ECommerceApp.Web.Controllers;

public class CartController : Controller
{
	private readonly ICartService _cartService;
	private readonly IOrderService _orderService;

	public CartController(ICartService cartService, IOrderService orderService)
	{
		_cartService = cartService;
		_orderService = orderService;
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

	public async Task<IActionResult> RemoveItem(int cartDetailsId)
	{
		var response = await _cartService.RemoveFromCartAsync(cartDetailsId);
		if(response != null && response.IsSuccess)
		{
			TempData["success"] = "Item Deleted Successfully";
			return RedirectToAction(nameof(Index));
		}

		return View();
	}

	public async Task<IActionResult> EmailCart(CartDto cartDto)
	{
		var cart = await LoadCart();
		cart.CartHeader.Email = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
		var response = await _cartService.EmailCart(cart);

		if(response != null && response.IsSuccess)
		{
			TempData["success"] = "Cart will be emailed shortly";
			return RedirectToAction(nameof(Index));
		}
		return View();
	}

	public async Task<IActionResult> Checkout()
	{
		var cart = await LoadCart();
		return View(cart);
	}

	[HttpPost]
	public async Task<IActionResult> Checkout(CartDto cartDto)
	{
		var cart = await LoadCart();
		cart.CartHeader.Name = cartDto.CartHeader.Name;
		cart.CartHeader.Phone = cartDto.CartHeader.Phone;
		cart.CartHeader.Email = cartDto.CartHeader.Email;


		var response = await _orderService.PlaceOrder(cart);
		if (response != null && response.IsSuccess)
		{
			var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

			var domain = Request.Scheme + "://" + Request.Host.Value + "/";

			StripeRequestDto stripeRequest = new StripeRequestDto()
			{
				ApprovedUrl = domain + "cart/confirmation?orderId=" + orderHeader.Id,
				CancelUrl = domain + "cart/checkout",
				OrderHeader = orderHeader
			};

			var stripeResponse = await _orderService.CreateStripeSession(stripeRequest);
			if(stripeResponse != null && stripeResponse.IsSuccess)
			{
				var stripeDto = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
				if(stripeDto != null && stripeDto.StripeSessionUrl != null)
					return Redirect(stripeDto.StripeSessionUrl);
			}
		}
		return View(cartDto);
	}

	public async Task<IActionResult> Confirmation(int orderId)
	{
		var response = await _orderService.ValidateStripeSession(orderId);

		if(response != null && response.IsSuccess)
		{
			var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
			if(orderHeader != null && orderHeader.PaymentStatus == StaticData.Status_Approved)
			{
				//await _cartService.ClearCartAsync(orderHeader.UserId);
				return View(orderId);
			}
		}	
		return RedirectToAction(nameof(Checkout));
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
