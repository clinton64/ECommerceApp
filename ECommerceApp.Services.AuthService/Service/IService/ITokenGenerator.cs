using Microsoft.AspNetCore.Identity;

namespace ECommerceApp.Services.AuthService.Service.IService;

public interface ITokenGenerator
{
	string GenerateToken(IdentityUser user, IList<string> roles);
}
