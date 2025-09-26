using ECommerceApp.Services.AuthService.Models.DTO;

namespace ECommerceApp.Services.AuthService.Service.IService;

public interface IAuthService
{
	Task<LoginResponse> Login(LoginRequest request);
	Task<string> Register(RegistrationRequest request);
}
