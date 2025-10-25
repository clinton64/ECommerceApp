namespace ECommerceApp.Web.Models;

public class OrderDto
{
	public OrderHeaderDto OrderHeader { get; set; }

	public IEnumerable<OrderDetailDto> OrderDetails { get; set; }
}
