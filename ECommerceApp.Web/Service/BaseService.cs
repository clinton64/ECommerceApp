using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static ECommerceApp.Web.Utility.StaticData;

namespace ECommerceApp.Web.Service;

public class BaseService : IBaseService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ITokenManager _tokenManager;

	public BaseService(IHttpClientFactory httpClientFactory, ITokenManager tokenManager)
	{
		_httpClientFactory = httpClientFactory;
		_tokenManager = tokenManager;
	}
	public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
	{
		HttpClient client = _httpClientFactory.CreateClient("ECommerceAPI");
		HttpRequestMessage message = new HttpRequestMessage();

		if (requestDto.ContentType == ContentType.MultipartFormData)
		{
			message.Headers.Add("Accept", "*/*");
		}
		else
		{
			message.Headers.Add("Accept", "application/json");
		}
		message.RequestUri = new Uri(requestDto.Url);

		// token
		if(withBearer)
		{
			message.Headers.Add("Authorization", $"Bearer {_tokenManager.GetToken()}");
		}

		if(requestDto.ContentType == ContentType.MultipartFormData)
		{
			var content = new MultipartFormDataContent();
			foreach(var prop in requestDto.Data.GetType().GetProperties())
			{
				var propValue = prop.GetValue(requestDto.Data);
				if (propValue is IFormFile file)
				{
					var streamContent = new StreamContent(file.OpenReadStream());
					streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
					content.Add(streamContent, prop.Name, file.FileName);
				}
				else
				{
					if (propValue != null)
						content.Add(new StringContent(propValue.ToString()), prop.Name);
				}
			}
			message.Content = content;
		}
		else
		{
			if (requestDto.Data != null)
			{
				message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
			}
		}

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
