namespace ECommerceApp.Services.AuthService.Models.DTO;

public class LoginResponse
{
	public UserDto User { get; set; }
	public string Token { get; set; }
}
