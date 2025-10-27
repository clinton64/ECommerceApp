using AutoMapper;
using ECommerceApp.Services.OrderService.Data;
using ECommerceApp.Services.OrderService.Messaging;
using ECommerceApp.Services.OrderService.Model;
using ECommerceApp.Services.OrderService.Model.DTO;
using ECommerceApp.Services.OrderService.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace ECommerceApp.Services.OrderService.Controllers;

[Route("api/order")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
	private ResponseDto _response;
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;
	private readonly IRabbitMQMessageSender _messageSender;
	private readonly IConfiguration _configuration;
	public OrderController(AppDbContext context, IMapper mapper, IRabbitMQMessageSender messageSender, IConfiguration configuration)
	{
		_response = new ResponseDto();
		_context = context;
		_mapper = mapper;
		_messageSender = messageSender;
		_configuration = configuration;
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

	[HttpPost("createstripesession")]
	public async Task<ResponseDto> CreateStripeSession(StripeRequestDto stripeRequest)
	{
		try
		{
			var options = new SessionCreateOptions
			{
				SuccessUrl = stripeRequest.ApprovedUrl,
				CancelUrl = stripeRequest.CancelUrl,
				LineItems = new List<SessionLineItemOptions>(),
				PaymentMethodTypes = new List<string>
				{
					"card"
				},
				Mode = "payment"
			};

			foreach (var item in stripeRequest.OrderHeader.OrderDetails)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.ProductName,
						},
						UnitAmount = (long)(item.Price * 100),
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);
			}
			var service = new SessionService();
			var session = service.Create(options);

			stripeRequest.StripeSessionUrl = session.Url;
			var orderHeader = _context.OrderHeaders.First(u => u.Id == stripeRequest.OrderHeader.Id);
			orderHeader.StripteSessionId = session.Id;
			await _context.SaveChangesAsync();

			_response.Result = stripeRequest;
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpGet("validatestripesession/{orderHeaderId}")]
	public async Task<ResponseDto> ValidateStripeSession(int orderHeaderId)
	{
		try
		{
			var orderHeader = _context.OrderHeaders.First(u => u.Id == orderHeaderId);

			var service = new SessionService();
			Session session = service.Get(orderHeader.StripteSessionId);

			var paymentIntentService = new PaymentIntentService();
			PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

			if (paymentIntent.Status.ToLower() == "succeeded")
			{
				orderHeader.OrderStatus = SD.Status_Approved;
				orderHeader.PaymentStatus = SD.Status_Approved;
				orderHeader.TransactionId = paymentIntent.Id;
				orderHeader.StripePaymentIntentId = paymentIntent.Id;
				await _context.SaveChangesAsync();

				_response.Result = _mapper.Map<OrderHeader>(orderHeader);

				// Send message to RabbitMQ 
				var queueName = _configuration.GetValue<string>("TopicAndQueueNames:OrderQueue");
				var orderDetails = _context.OrderDetails.Where(o => o.OrderHeaderId == orderHeader.Id).ToList();
				var orderHeaderDto = _mapper.Map<OrderHeaderDto>(orderHeader);
				orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);
				_messageSender.SendMessage(orderHeaderDto, queueName);
			}
		}
		catch( Exception ex)
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
