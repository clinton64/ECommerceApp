using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using ECommerceApp.Web.Utility;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static ECommerceApp.Web.Utility.StaticData;

namespace ECommerceApp.Web.Service;

public class BaseService : IBaseService
{
	private readonly IHttpClientFactory _httpClientFactory;

	public BaseService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}
	public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
	{
		HttpClient client = _httpClientFactory.CreateClient("ECommerceAPI");
		HttpRequestMessage message = new HttpRequestMessage();

		message.Headers.Add("Accept", "application/json");
		message.RequestUri = new Uri(requestDto.Url);

		// token

		switch (requestDto.ApiType)
		{
			case ApiType.POST:
				message.Method = HttpMethod.Post;
				break;
			case ApiType.PUT:
				message.Method = HttpMethod.Put;
				break;
			case ApiType.DELETE:
				message.Method = HttpMethod.Delete;
				break;
			default:
				message.Method = HttpMethod.Get;
				break;
		}

		if (requestDto.Data != null)
		{
			message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
		}

		HttpResponseMessage? apiResponse = await client.SendAsync(message);

		try
		{
			switch (apiResponse.StatusCode)
			{
				case HttpStatusCode.NotFound:
					return new() { IsSuccess = false, Message = "Not Found" };
				case HttpStatusCode.Unauthorized:
					return new() { IsSuccess = false, Message = "Unauthorized" };
				case HttpStatusCode.Forbidden:
					return new() { IsSuccess = false, Message = "Access denied" };
				case HttpStatusCode.InternalServerError:
					return new() { IsSuccess = false, Message = "Internal Server Error" };
				default:
					var apiContent = await apiResponse.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<ResponseDto>(apiContent);

			}
		}
		catch (Exception ex)
		{
			return new ResponseDto
			{
				IsSuccess = false,
				Message = ex.Message
			};
		}
	}
}
