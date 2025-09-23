namespace ECommerceApp.Web.Utility;

public static class StaticData
{
	public static string CouponAPIBase = "https://localhost:7001";
	public enum ApiType
	{
		GET,
		POST,
		PUT,
		DELETE
	}
	public enum ContentType
	{
		JSON,
		MultipartFormData
	}
}
