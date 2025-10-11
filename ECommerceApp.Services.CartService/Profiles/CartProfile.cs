using AutoMapper;
using ECommerceApp.Services.CartService.Models;
using ECommerceApp.Services.CartService.Models.DTO;

namespace ECommerceApp.Services.CartService.Profiles;

public class CartProfile : Profile
{
	public CartProfile()
	{
		CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
		CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
	}
}