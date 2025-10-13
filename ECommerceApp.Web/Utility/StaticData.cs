namespace ECommerceApp.Web.Utility;

public static class StaticData
{
	public static string CouponAPIBase = "https://localhost:7001";
	public static string AuthAPIBase = "https://localhost:7002";
	public static string ProductAPIBase = "https://localhost:7003";
	public static string CartAPIBase = "https://localhost:7004";
	public const string TokenCookie = "JWTToken";
	public const string Role_User_Admin = "Admin";

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
