using AutoMapper;
using ECommerceApp.Services.CouponService.Models;
using ECommerceApp.Services.CouponService.Models.DTO;

namespace ECommerceApp.Services.CouponService.Profiles;

public class CouponProfile : Profile
{
	public CouponProfile()
	{
		CreateMap<Coupon, CouponDto>().ReverseMap();
	}
}