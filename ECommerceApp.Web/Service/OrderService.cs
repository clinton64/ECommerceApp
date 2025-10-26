using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using static ECommerceApp.Web.Utility.StaticData;

namespace ECommerceApp.Web.Service;

public class OrderService : IOrderService
{
	private readonly IBaseService _baseService;

	public OrderService(IBaseService baseService)
	{
		_baseService = baseService;
	}
	public async Task<ResponseDto?> GetOrder(int orderId)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = OrderAPIBase + "/api/order/get/" + orderId,
		});
	}

	public async Task<ResponseDto?> GetOrders()
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = OrderAPIBase + "/api/order/get",
		});
	}

	public async Task<ResponseDto?> PlaceOrder(CartDto cart)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.POST,
			Url = OrderAPIBase + "/api/order/create",
			Data = cart
		});
	}

	public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequest)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.POST,
			Url = OrderAPIBase + "/api/order/createstripesession",
			Data = stripeRequest
		});
	}

	public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = OrderAPIBase + "/api/order/validatestripesession/" + orderHeaderId,
		});
	}
}

