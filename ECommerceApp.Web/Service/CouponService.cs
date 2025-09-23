using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using static ECommerceApp.Web.Utility.StaticData;

namespace ECommerceApp.Web.Service;

public class CouponService : ICouponService
{
	private readonly IBaseService _baseService;
	public CouponService(IBaseService baseService)
	{
		_baseService = baseService;
	}
	public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponCreateDto)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType= ApiType.POST,
			Url = CouponAPIBase + "/api/coupon",
			Data = couponCreateDto
		});
	}

	public async Task<ResponseDto?> DeleteCouponAsync(int id)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.DELETE,
			Url = CouponAPIBase + "/api/coupon/" + id,
		});
	}

	public async Task<ResponseDto?> GetAllCouponsAsycn()
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = CouponAPIBase + "/api/coupon",
		});	
	}

	public async Task<ResponseDto?> GetCouponAsync(string couponCode)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = CouponAPIBase + "/api/coupon/GetByCode/" + couponCode
		});
	}

	public async Task<ResponseDto?> GetCouponAsync(int id)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = CouponAPIBase + "/api/coupon/" + id
		});
	}

	public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponUpdateDto)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.PUT,
			Url = CouponAPIBase + "/api/coupon",
			Data = couponUpdateDto
		});
	}
}
