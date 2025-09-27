using ECommerceApp.Web.Models;

namespace ECommerceApp.Web.Service.IService;

public interface IAuthService
{
	Task<ResponseDto?> LoginAsync(LoginRequest obj);
	Task<ResponseDto?> RegisterAsync(RegisterRequest obj);
}
