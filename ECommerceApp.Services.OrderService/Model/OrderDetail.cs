using ECommerceApp.Services.OrderService.Model.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Services.OrderService.Model;

public class OrderDetail
{
	[Key]
	public int Id { get; set; }

	public int OrderHeaderId { get; set; }

	[ForeignKey(nameof(OrderHeaderId))]
	public OrderHeader OrderHeader { get; set; }


	public int ProductId { get; set; }

	[NotMapped]
	public ProductDto Product { get; set; }

	public int Count { get; set; }

	public string ProductName { get; set; }

	public double Price { get; set; }	
}
