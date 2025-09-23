using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerceApp.Web.Controllers;

public class CouponController : Controller
{
	private readonly ICouponService _couponService;

	public CouponController(ICouponService couponService)
	{
		_couponService = couponService;
	}
	public async Task<IActionResult> Index()
	{
		List<CouponDto> couponList = new();

		var response = await _couponService.GetAllCouponsAsycn();
		if(response != null && response.IsSuccess)
		{
			couponList = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
		}
		else
		{
			TempData["error"] = response?.Message;
		}

		return View(couponList);
	}

	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Create(CouponDto model)
	{
		if (ModelState.IsValid)
		{
			var response = await _couponService.CreateCouponAsync(model);
			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
		}
		return View(model);
	}

	public async Task<IActionResult> Delete(int id)
	{
		var response = await _couponService.GetCouponAsync(id);
		if (response != null && response.IsSuccess)
		{
			var coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
			return View(coupon);
		}
		else
		{
			TempData["error"] = response?.Message;
		}
		return NotFound();
	}

	[HttpPost]
	public async Task<IActionResult> Delete(CouponDto model)
	{
		var response = await _couponService.DeleteCouponAsync(model.Id);
		if (response != null && response.IsSuccess)
		{
			TempData["success"] = "Coupon deleted successfully";	
			return RedirectToAction(nameof(Index));
		}
		else
		{
			TempData["error"] = response?.Message;
		}
		return View(model);
	}
}
