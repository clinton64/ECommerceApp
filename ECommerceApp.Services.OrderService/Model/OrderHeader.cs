using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Services.OrderService.Model;

public class OrderHeader
{
	[Key]
	public int Id { get; set; }	
	public string? UserId { get; set; }
	public string? CouponCode { get; set; }	
	public DateTime OrderDate { get; set; }
	public double Discount { get; set; }
	public double OrderTotal { get; set; }


	public string? OrderStatus { get; set; }
	public string? PaymentStatus { get; set; }
	public string? TransactionId { get; set; }
	public string? StripePaymentIntentId { get; set; }
	public string? StripteSessionId { get; set; }

	public IEnumerable<OrderDetail> OrderDetails { get; set; }

	public string? Name { get; set; }
	public string? Phone { get; set; }
	public string? Email { get; set; }
	public string? Address { get; set; } = string.Empty;
}
