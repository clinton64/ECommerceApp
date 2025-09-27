namespace ECommerceApp.Web.Service.IService;

public interface ITokenManager
{
	void SetToken(string token);
	string? GetToken();
	void ClearToken();
}
