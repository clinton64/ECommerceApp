using AutoMapper;
using ECommerceApp.Services.CouponService.Data;
using ECommerceApp.Services.CouponService.Models.DTO;
using ECommerceApp.Services.CouponService.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Services.CouponService.Controllers;

[ApiController]
[Route("api/coupon")]
[Authorize]
public class CouponController : Controller
{
	private readonly AppDBContext _context;
	private readonly IMapper _mapper;
	private ResponseDto _response;

	public CouponController(AppDBContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;	
		_response = new ResponseDto();
	}
	[HttpGet]
	public ResponseDto Get()
	{
		try
		{
			var objList = _context.Coupons.ToList();
			_response.Result = _mapper.Map<List<CouponDto>>(objList);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpGet]
	[Route("{id:int}")]
	public ResponseDto Get(int id)
	{
		try
		{
			var obj = _context.Coupons.First(u => u.Id == id);
			_response.Result = _mapper.Map<CouponDto>(obj);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpGet]
	[Route("GetByCode/{code}")]
	public ResponseDto GetByCode(string code)
	{
		try
		{
			var obj = _context.Coupons.First(u => u.CouponCode.ToLower() == code.ToLower());
			_response.Result = _mapper.Map<CouponDto>(obj);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpPost]
	[Authorize(Roles = SD.Role_User_Admin)]
	public ResponseDto Post([FromBody] CouponDto couponDto)
	{
		try
		{
			var obj = _mapper.Map<Models.Coupon>(couponDto);
			_context.Coupons.Add(obj);
			_context.SaveChanges();
			_response.Result = _mapper.Map<CouponDto>(obj);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpPut]
	[Authorize(Roles = SD.Role_User_Admin)]
	public ResponseDto Put([FromBody] CouponDto couponDto)
	{
		try
		{
			_context.Coupons.First(u => u.Id == couponDto.Id);  // throw exception if not exists

			var obj = _mapper.Map<Models.Coupon>(couponDto);
			_context.Coupons.Update(obj);
			_context.SaveChanges();
			_response.Result = _mapper.Map<CouponDto>(obj);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}

	[HttpDelete]
	[Route("{id:int}")]
	[Authorize(Roles = SD.Role_User_Admin)]
	public ResponseDto Delete(int id)
	{
		try
		{
			var obj = _context.Coupons.First(u => u.Id == id);
			_context.Coupons.Remove(obj);
			_context.SaveChanges();
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.Message = ex.Message;
		}
		return _response;
	}
}
