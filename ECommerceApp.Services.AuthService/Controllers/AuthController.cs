using ECommerceApp.Services.AuthService.Models.DTO;
using ECommerceApp.Services.AuthService.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Services.AuthService.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private ResponseDto _response;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
			_response = new ();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var loginResponse = await _authService.Login(request);	
			if (loginResponse.User == null)
			{
				_response.IsSuccess = false;
				_response.Message = "Username or password is incorrect";
				return BadRequest(_response);
			}
			_response.Result = loginResponse;
			return Ok(_response) ;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegistrationRequest request)
		{
			var errorMessage = await _authService.Register(request);
			if (!string.IsNullOrEmpty(errorMessage))
			{
				_response.IsSuccess = false;
				_response.Message = errorMessage;
				return BadRequest(_response);
			}
			//await _messageBus.PublishMessage(request.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
			return Ok(_response);
		}
	}
}
