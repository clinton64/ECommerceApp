using AutoMapper;
using ECommerceApp.Services.OrderService.Model;
using ECommerceApp.Services.OrderService.Model.DTO;

namespace ECommerceApp.Services.OrderService.Profiles;

public class OrderProfile : Profile
{
	public OrderProfile()
	{
		CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
		CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();

		CreateMap<OrderHeaderDto, CartHeaderDto>()
			.ForMember(m => m.CartTotal, m => m.MapFrom(src => src.OrderTotal)).ReverseMap();

		CreateMap<CartDetailsDto, OrderDetailDto>()
			.ForMember(m => m.ProductName, m => m.MapFrom(src => src.Product.Name))
			.ForMember(m => m.Price, m => m.MapFrom(src => src.Product.Price));

		CreateMap<OrderDetailDto, CartDetailsDto>()
	}
}
