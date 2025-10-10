
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace ECommerceApp.Services.CartService.Service;

public class HttpClientHandler: DelegatingHandler
{
	private readonly IHttpContextAccessor _contextAccessor;
	
	public HttpClientHandler(IHttpContextAccessor contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var token = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		
		return await base.SendAsync(request, cancellationToken);
	}
}
