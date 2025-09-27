using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using ECommerceApp.Web.Utility;

namespace ECommerceApp.Web.Service;

public class AuthService : IAuthService
{
	private readonly IBaseService _baseService;
	public AuthService(IBaseService baseService)
	{
		_baseService = baseService;
	}
	public async Task<ResponseDto?> LoginAsync(LoginRequest request)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = StaticData.ApiType.POST,
			Data = request,
			Url = StaticData.AuthAPIBase + "/api/auth/login"
		}, withBearer: false);
	}

	public async Task<ResponseDto?> RegisterAsync(RegisterRequest request)
	{
		return await _baseService.SendAsync(new RequestDto
		{
			ApiType = StaticData.ApiType.POST,
			Data = request,
			Url = StaticData.AuthAPIBase + "/api/auth/register"
		}, withBearer: false);
	}
}
