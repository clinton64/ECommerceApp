using ECommerceApp.Services.CartService.Models.DTO;
using ECommerceApp.Services.CartService.Service.IService;
using Newtonsoft.Json;

namespace ECommerceApp.Services.CartService.Service;

public class ProductService : IProductService
{
	private IHttpClientFactory _httpClientFactory;
	public ProductService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}
	public async Task<IEnumerable<ProductDto>> GetProducts()
	{
		var client = _httpClientFactory.CreateClient("Product");
		var apiRresponse = await client.GetAsync($"/api/product");
		var response = JsonConvert.DeserializeObject<ResponseDto>(await apiRresponse.Content.ReadAsStringAsync());
		
		if(response != null && response.IsSuccess)
		{
			return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(response.Result));
		}
		return new List<ProductDto>();
	}
}
