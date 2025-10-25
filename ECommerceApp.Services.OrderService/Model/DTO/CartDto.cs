namespace ECommerceApp.Services.OrderService.Model.DTO;

public class CartDto
{
	public CartHeaderDto CartHeader { get; set; }

	public IEnumerable<CartDetailsDto>? CartDetails { get; set; }	
}
