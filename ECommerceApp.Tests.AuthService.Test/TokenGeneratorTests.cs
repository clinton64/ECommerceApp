using ECommerceApp.Services.AuthService.Service;
using ECommerceApp.Services.AuthService.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceApp.Tests.AuthService.Test;

public class TokenGeneratorTests
{
	private readonly ITokenGenerator _tokenGenerator;
	private readonly Mock<IConfiguration> _configurationMock;

	public TokenGeneratorTests()
	{
		// Mock Configuration
		_configurationMock = new Mock<IConfiguration>();

		_configurationMock.Setup(c => c["Jwt:Key"]).Returns("xXCoYQaMftG7O1Po+pGkJPZ6xzDf2R2RqhPmbrTSYdA=");
		_configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testIssuer");
		_configurationMock.Setup(c => c["Jwt:Audience"]).Returns("testAudience");
		_configurationMock.Setup(c => c["Jwt:ExpireDays"]).Returns("1");

		_tokenGenerator = new TokenGenerator(_configurationMock.Object);
	}

	[Fact]
	public void GenerateToken_ShouldReturn_ValidJwtToken()
	{
		// Arrange
		var user = CreateTestUser();
		var roles = new List<string> { "Admin", "User" };

		// Act
		var token = _tokenGenerator.GenerateToken(user, roles);

		// Assert
		Assert.False(string.IsNullOrEmpty(token));

		// Validate token
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes(_configurationMock.Object["Jwt:Key"]);

		tokenHandler.ValidateToken(token, new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ValidateIssuer = true,
			ValidIssuer = _configurationMock.Object["Jwt:Issuer"],
			ValidateAudience = true,
			ValidAudience = _configurationMock.Object["Jwt:Audience"],
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		}, out var validatedToken);

		var jwtToken = (JwtSecurityToken)validatedToken;

		// verify claims
		Assert.Equal(user.Id, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
		Assert.Equal(user.UserName, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
		Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);

		// verify roles
		var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
		Assert.Contains("Admin", roleClaims);
		Assert.Contains("User", roleClaims);
	}

	[Fact]
	public void ValidateToken_InvalidKey_ShouldThrowException()
	{
		var user = CreateTestUser();
		var token = _tokenGenerator.GenerateToken(user, new List<string> { "Manager" });

		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.UTF8.GetBytes("wrong_secret_key");

		Assert.ThrowsAny<SecurityTokenException>(() =>
		{
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = _configurationMock.Object["Jwt:Issuer"],
				ValidateAudience = true,
				ValidAudience = _configurationMock.Object["Jwt:Audience"],
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			}, out _);
		});
		;

	}

	[Fact]
	public void ValidateToken_ExpiredToken_ShouldThrowException()
	{
		var user = CreateTestUser();
		var key = Encoding.UTF8.GetBytes(_configurationMock.Object["Jwt:Key"]);

		var expiredToken = new JwtSecurityToken(
			   issuer: _configurationMock.Object["Jwt:Issuer"],
			   audience: _configurationMock.Object["Jwt:Audience"],
			   claims: new[]
			   {
					new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Id)
			   },
			   notBefore: DateTime.UtcNow.AddMinutes(-10),
			   expires: DateTime.UtcNow.AddMinutes(-1), // already expired
			   signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
		   );

		var expiredTokenString = new JwtSecurityTokenHandler().WriteToken(expiredToken);
		var tokenHandler = new JwtSecurityTokenHandler();

		// Act & Assert
		Assert.Throws<SecurityTokenExpiredException>(() =>
		{
			tokenHandler.ValidateToken(expiredTokenString, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = _configurationMock.Object["Jwt:Issuer"],
				ValidateAudience = true,
				ValidAudience = _configurationMock.Object["Jwt:Audience"],
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			}, out _);
		});
		;

	}

	private IdentityUser CreateTestUser() => new IdentityUser
	{
		Id = "test-user-id",
		UserName = "test-username",
		Email = "test-email"
	};

}