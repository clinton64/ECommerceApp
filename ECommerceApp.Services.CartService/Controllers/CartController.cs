using AutoMapper;
using ECommerceApp.Services.CartService.Data;
using ECommerceApp.Services.CartService.Models;
using ECommerceApp.Services.CartService.Models.DTO;
using ECommerceApp.Services.CartService.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.CartService.Controllers;

[Route("api/cart")]
[ApiController]
public class CartController : ControllerBase
{
	private ResponseDto _response;
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;
	private readonly IProductService _productService;
	private readonly ICouponService _couponService;

	public CartController(AppDbContext context, IMapper mapper, IProductService productService, ICouponService couponService)
	{
		_response = new();
		_context = context;
		_mapper = mapper;
		_productService = productService;
		_couponService = couponService;
	}
	[HttpGet("GetCart/{userId}")]
	public async Task<ResponseDto> GetCart(string userId)
	{
		try
		{
			CartDto cart = new()
			{
				CartHeader = _mapper.Map<CartHeaderDto>(_context.CartHeaders.First(c => c.UserId == userId)),
			};

			cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails.Where(c => c.CartHeaderId == cart.CartHeader.Id));

			var products = await _productService.GetProducts();

			foreach(var item in cart.CartDetails)
			{
				item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
				cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
			}

			// apply coupon if any
			if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
			{
				var coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
				if (coupon != null && cart.CartHeader.CartTotal > coupon.MinimumAmount)
				{
					cart.CartHeader.CartTotal -= coupon.DiscountAmount;
					cart.CartHeader.Discount = coupon.DiscountAmount;
				}
			}
			_response.Result = cart;
		}
		catch(Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;	
		}
		return _response;
	}

	[HttpPost("ApplyCoupon")]
	public async Task<ResponseDto> ApplyCoupon(CartDto cart)
	{
		try
		{
			var cartDb = _context.CartHeaders.FirstOrDefault(c => c.UserId == cart.CartHeader.UserId);
			cartDb.CouponCode = cart.CartHeader.CouponCode;
			
			_context.CartHeaders.Update(cartDb);
			await _context.SaveChangesAsync();
			_response.Result = true;
		}
		catch (Exception ex) 
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpPost("Upsert")]
	public async Task<ResponseDto> Upsert(CartDto cart)
	{
		try
		{
			// Find existing CartHeader for this user
			var cartHeaderDb = await _context.CartHeaders
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

			if (cartHeaderDb == null)
			{
				// Create new CartHeader
				var newHeader = _mapper.Map<CartHeader>(cart.CartHeader);
				_context.CartHeaders.Add(newHeader);
				await _context.SaveChangesAsync();

				// Add the CartDetail linked to new header
				var newDetail = _mapper.Map<CartDetails>(cart.CartDetails.First());
				newDetail.CartHeaderId = newHeader.Id;
				newDetail.CartHeader = null; // prevent EF from inserting a new header
				_context.CartDetails.Add(newDetail);

				await _context.SaveChangesAsync();
			}
			else
			{
				var incomingDetail = cart.CartDetails.First();

				// Check if the product already exists in the user's cart
				var existingDetail = await _context.CartDetails
					.FirstOrDefaultAsync(cd => cd.CartHeaderId == cartHeaderDb.Id &&
											   cd.ProductId == incomingDetail.ProductId);

				if (existingDetail == null)
				{
					// Add new product to cart
					var newDetail = _mapper.Map<CartDetails>(incomingDetail);
					newDetail.CartHeaderId = cartHeaderDb.Id;
					newDetail.CartHeader = null;
					_context.CartDetails.Add(newDetail);
				}
				else
				{
					// Update quantity for existing product
					existingDetail.Count += incomingDetail.Count;
					_context.CartDetails.Update(existingDetail);
				}

				await _context.SaveChangesAsync();
			}

			_response.Result = cart;
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}

		return _response;
	}

}

