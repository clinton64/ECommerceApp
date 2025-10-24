using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using static ECommerceApp.Web.Utility.StaticData;

namespace ECommerceApp.Web.Service;

public class CartService : ICartService
{
	private readonly IBaseService _baseService;

	public CartService(IBaseService baseService)
	{
		_baseService = baseService;
	}
	public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.POST,
			Data = cartDto,
			Url = CartAPIBase + "/api/cart/ApplyCoupon"
		});
	}

	public async Task<ResponseDto?> EmailCart(CartDto cartDto)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.POST,
			Data = cartDto,
			Url = CartAPIBase + "/api/cart/EmailCartRequest"
		});
	}

	public async Task<ResponseDto?> GetCartByUserIdAsnyc(string userId)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = CartAPIBase + $"/api/cart/GetCart/{userId}"
		});
	}

	public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = CartAPIBase +  $"/api/cart/DeleteItem/{cartDetailsId}"
		});
	}

	public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.POST,
			Data = cartDto,
			Url = CartAPIBase + "/api/cart/Upsert"
		});
	}
}
