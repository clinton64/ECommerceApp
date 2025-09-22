using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Services.CouponService.Models;

public class Coupon
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string CouponCode { get; set; } = string.Empty;

	[Required]
	public int DiscountAmount { get; set; }

	public int MinimumAmount { get; set; }
}
