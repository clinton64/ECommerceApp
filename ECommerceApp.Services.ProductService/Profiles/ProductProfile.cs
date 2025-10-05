using AutoMapper;
using ECommerceApp.Services.ProductService.Models;
using ECommerceApp.Services.ProductService.Models.DTO;

namespace ECommerceApp.Services.ProductService.Profiles;

public class ProductProfile : Profile
{
	public ProductProfile()
	{
		CreateMap<Product, ProductDto>().ReverseMap();
	}
}