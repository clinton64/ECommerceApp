namespace ECommerceApp.Services.OrderService.Model.DTO;

public class OrderDto
{
	public OrderHeaderDto OrderHeader { get; set; }

	public IEnumerable<OrderDetailDto> OrderDetails { get; set; }
}
