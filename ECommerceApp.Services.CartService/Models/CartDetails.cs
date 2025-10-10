using ECommerceApp.Services.CartService.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Services.CartService.Models;

public class CartDetails
{
	[Key]
	public int Id { get; set; }

	public int CartHeaderId{ get; set; }
	[ForeignKey("CartHeaderId")]
	public CartHeader CartHeader { get; set; }
	
	public int ProductId{ get; set; }
	[NotMapped]
	public ProductDto Product { get; set; }
	public int Count { get; set; }	
}
