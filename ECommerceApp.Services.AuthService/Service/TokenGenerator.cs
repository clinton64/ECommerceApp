using ECommerceApp.Services.AuthService.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceApp.Services.AuthService.Service;

public class TokenGenerator: ITokenGenerator
{
	private readonly IConfiguration _configuration;
	public TokenGenerator(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	public string GenerateToken(IdentityUser user, IList<string> roles)
	{
		var utcNow = DateTime.UtcNow;

		var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),	
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
			};

		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

		var jwt = new JwtSecurityToken(
			claims: claims,
			expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(jwt);
	}	
}
