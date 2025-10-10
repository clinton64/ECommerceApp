using ECommerceApp.Services.CartService.Models.DTO;

namespace ECommerceApp.Services.CartService.Service.IService;

public interface ICouponService
{
	Task<CouponDto> GetCoupon(string couponCode);
}
