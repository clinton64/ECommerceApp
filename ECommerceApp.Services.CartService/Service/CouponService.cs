using ECommerceApp.Services.CartService.Models.DTO;
using ECommerceApp.Services.CartService.Service.IService;
using Newtonsoft.Json;

namespace ECommerceApp.Services.CartService.Service;

public class CouponService : ICouponService
{
	private IHttpClientFactory _httpClientFactory;

	public CouponService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}
	public async Task<CouponDto> GetCoupon(string couponCode)
	{
		var client = _httpClientFactory.CreateClient("Coupon");
		var apiResponse = await client.GetAsync($"api/coupon/{couponCode}");
		var response = JsonConvert.DeserializeObject<ResponseDto>(await apiResponse.Content.ReadAsStringAsync());
		
		if(response != null && response.IsSuccess)
		{
			return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
		}
		return new CouponDto();
	}
}
