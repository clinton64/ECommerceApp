using ECommerceApp.Web.Service.IService;
using ECommerceApp.Web.Utility;

namespace ECommerceApp.Web.Service;

public class TokenManager : ITokenManager
{
	private readonly IHttpContextAccessor _contextAccessor;

	public TokenManager(IHttpContextAccessor contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}


	public void ClearToken()
	{
		_contextAccessor.HttpContext?.Response.Cookies.Delete(StaticData.TokenCookie);
	}

	public string? GetToken()
	{
		string? token = null;
		bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticData.TokenCookie, out token);
		return hasToken is true ? token : null;
	}

	public void SetToken(string token)
	{
		_contextAccessor.HttpContext?.Response.Cookies.Append(StaticData.TokenCookie, token);
	}
}
