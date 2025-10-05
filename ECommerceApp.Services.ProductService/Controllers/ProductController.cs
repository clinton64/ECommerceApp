using AutoMapper;
using ECommerceApp.Services.CouponService.Utility;
using ECommerceApp.Services.ProductService.Data;
using ECommerceApp.Services.ProductService.Models;
using ECommerceApp.Services.ProductService.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Services.ProductService.Controllers
{
	[Route("api/Product")]
	[ApiController]
	[Authorize]
	public class ProductController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;
		private ResponseDto _response;

		public ProductController(AppDbContext context, IMapper mapper)
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
				var objList = _context.Products.ToList();
				_response.Result = _mapper.Map<List<ProductDto>>(objList);
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
				var obj = _context.Products.First(u => u.Id == id);
				_response.Result = _mapper.Map<ProductDto>(obj);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + ","  + SD.Role_User_Manager)]
		public ResponseDto Post(ProductDto productDto)
		{
			try
			{
				var product = _mapper.Map<Product>(productDto);
				_context.Products.Add(product);
				_context.SaveChanges();

				if (productDto.Image != null)
				{
					var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");
					if (!Directory.Exists(uploadDir))
					{
						Directory.CreateDirectory(uploadDir);
					}

					string fileName = product.Id + Path.GetExtension(productDto.Image.FileName);
					string filePath = Path.Combine(uploadDir, fileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						productDto.Image.CopyTo(stream);
					}

					var baseUrl = $"{Request.Scheme}://{Request.Host}";
					product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
					product.ImageLocalPath = filePath;

					_context.Products.Update(product);
					_context.SaveChanges();
				}

				_response.Result = _mapper.Map<ProductDto>(product);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPut]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Manager)]
		public ResponseDto Put(ProductDto productDto)
		{
			try
			{
				var productDb = _context.Products.First(u => u.Id == productDto.Id);
				_mapper.Map(productDto, productDb);

				if (productDto.Image != null)
				{
					var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");
					if (!Directory.Exists(uploadDir))
					{
						Directory.CreateDirectory(uploadDir);
					}

					string fileName = productDb.Id + Path.GetExtension(productDto.Image.FileName);
					string filePath = Path.Combine(uploadDir, fileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						productDto.Image.CopyTo(stream);
					}

					var baseUrl = $"{Request.Scheme}://{Request.Host}";
					productDb.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
					productDb.ImageLocalPath = filePath;

					_context.Products.Update(productDb);
					_context.SaveChanges();
				}

				_context.SaveChanges();
				_response.Result = _mapper.Map<ProductDto>(productDb);
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
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Manager)]
		public ResponseDto Delete(int id)
		{
			try
			{
				var obj = _context.Products.First(u => u.Id == id);
				if(! string.IsNullOrEmpty(obj.ImageLocalPath))
				{
					var directory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
					var existingFile = new FileInfo(directory);
					if (existingFile.Exists)
					{
						existingFile.Delete();
					}
				}
				_context.Products.Remove(obj);
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
}
