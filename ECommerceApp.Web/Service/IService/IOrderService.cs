using ECommerceApp.Web.Models;

namespace ECommerceApp.Web.Service.IService;

public interface IOrderService
{
	Task<ResponseDto?> GetOrders();
	Task<ResponseDto?> GetOrder(int orderId);
	Task<ResponseDto?> PlaceOrder(CartDto cart);
	Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequest);
	Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
}
