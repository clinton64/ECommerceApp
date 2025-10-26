namespace ECommerceApp.Web.Utility;

public static class StaticData
{
	public static string CouponAPIBase = "https://localhost:7001";
	public static string AuthAPIBase = "https://localhost:7002";
	public static string ProductAPIBase = "https://localhost:7003";
	public static string CartAPIBase = "https://localhost:7004";
	public static string OrderAPIBase = "https://localhost:7005";
	public const string TokenCookie = "JWTToken";
	public const string Role_User_Admin = "Admin";

	public const string Status_Pending = "Pending";
	public const string Status_Approved = "Approved";
	public const string Status_ReadyForPickup = "ReadyForPickup";
	public const string Status_Completed = "Completed";
	public const string Status_Refunded = "Refunded";
	public const string Status_Cancelled = "Cancelled";

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
