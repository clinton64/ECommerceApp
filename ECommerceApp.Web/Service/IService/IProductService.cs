using ECommerceApp.Web.Models;

namespace ECommerceApp.Web.Service.IService;

public interface IProductService
{
	Task<ResponseDto?> GetAllProductsAsycn();
	Task<ResponseDto?> GetProductAsync(int id);
	Task<ResponseDto?> CreateProductAsync(ProductDto productCreateDto);
	Task<ResponseDto?> UpdateProductAsync(ProductDto productUpdateDto);
	Task<ResponseDto?> DeleteProductAsync(int id);
}
