using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using static ECommerceApp.Web.Utility.StaticData;

namespace ECommerceApp.Web.Service;

public class ProductService : IProductService
{
	private readonly IBaseService _baseService;
	public ProductService(IBaseService baseService) { 
		_baseService = baseService;
	}
	public async Task<ResponseDto?> CreateProductAsync(ProductDto productCreateDto)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.POST,
			Url = ProductAPIBase + "/api/product",
			Data = productCreateDto,
			ContentType = ContentType.MultipartFormData
		});
	}

	public async Task<ResponseDto?> DeleteProductAsync(int id)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.DELETE,
			Url = ProductAPIBase + "/api/product/" + id
		});
	}

	public async Task<ResponseDto?> GetAllProductsAsycn()
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = ProductAPIBase + "/api/product"
		});
	}

	public async Task<ResponseDto?> GetProductAsync(int id)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.GET,
			Url = ProductAPIBase + "/api/product/" + id
		});
	}

	public async Task<ResponseDto?> UpdateProductAsync(ProductDto productUpdateDto)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = ApiType.PUT,
			Url = ProductAPIBase + "/api/product",
			Data = productUpdateDto,
			ContentType = ContentType.MultipartFormData
		});
	}
}
