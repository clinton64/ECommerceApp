namespace ECommerceApp.Web.Models;

public class CouponDto
{
	public int Id { get; set; }

	public string CouponCode { get; set; } = string.Empty;

	public int DiscountAmount { get; set; }

	public int MinimumAmount { get; set; }
}