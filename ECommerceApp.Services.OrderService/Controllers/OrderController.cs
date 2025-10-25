using AutoMapper;
using ECommerceApp.Services.OrderService.Data;
using ECommerceApp.Services.OrderService.Model;
using ECommerceApp.Services.OrderService.Model.DTO;
using ECommerceApp.Services.OrderService.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.OrderService.Controllers;

[Route("api/order")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
	private ResponseDto _response;
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;
	public OrderController(AppDbContext context, IMapper mapper)
	{
		_response = new ResponseDto();
		_context = context;
		_mapper = mapper;
	}

	[HttpGet("get")]
	public async Task<ResponseDto> Get()
	{
		try
		{
			IEnumerable<OrderHeader> orders;
			if (User.IsInRole(SD.RoleAdmin))
			{
				orders = _context.OrderHeaders.Include(u => u.OrderDetails)
					.OrderByDescending(o => o.Id).ToList();
			}
			else
			{
				orders = _context.OrderHeaders.Include(u => u.OrderDetails)
					.Where(u => u.UserId == User.Identity.Name)
					.OrderByDescending(o => o.Id).ToList();
			}
			_response.Result = _mapper.Map<IEnumerable<OrderDto>>(orders);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpGet("get/{orderId}")]
	public async Task<ResponseDto>? Get(int orderId)
	{
		try
		{
			var order = _context.OrderHeaders.Include(u => u.OrderDetails)
				.First(o => o.Id == orderId);
			_response.Result = _mapper.Map<OrderDto>(order);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpPost("create")]
	public async Task<ResponseDto> CreateOrder(CartDto cart)
	{
		try
		{
			var orderHeaderDto = _mapper.Map<OrderHeaderDto>(cart.CartHeader);
			orderHeaderDto.OrderDate = DateTime.Now;
			orderHeaderDto.OrderStatus = SD.Status_Pending;
			orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDto>>(cart.CartDetails);

			var orderHeader = _context.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
			await _context.SaveChangesAsync();

			orderHeaderDto.Id = orderHeader.Id;
			_response.Result = orderHeaderDto;
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpPost("update/{orderId}")]	
	public async Task<ResponseDto> UpdateOrderStatus(int orderId, string status)
	{
		try
		{
			var order = _context.OrderHeaders.First(o => o.Id == orderId);
			if(order != null)
			{
				if(status == SD.Status_Cancelled)
				{
					// Refund logic can be added here
				}

				order.OrderStatus = status;
				await _context.SaveChangesAsync();
			}
			else
			{
				_response.IsSuccess = false;
				_response.Message = "Order not found.";
			}
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}
}
