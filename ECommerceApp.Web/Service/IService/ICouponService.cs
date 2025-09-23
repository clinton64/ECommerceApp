using ECommerceApp.Web.Models;

namespace ECommerceApp.Web.Service.IService;

public interface ICouponService
{
	Task<ResponseDto?> GetAllCouponsAsycn();
	Task<ResponseDto?> GetCouponAsync(string couponCode);
	Task<ResponseDto?> GetCouponAsync(int id);
	Task<ResponseDto?> CreateCouponAsync(CouponDto couponCreateDto);
	Task<ResponseDto?> UpdateCouponAsync(CouponDto couponUpdateDto);
	Task<ResponseDto?> DeleteCouponAsync(int id);
}
