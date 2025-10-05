using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace ECommerceApp.Web.Controllers;

public class ProductController : Controller
{
	private readonly IProductService _productService;
	public ProductController(IProductService productService)
	{
		_productService = productService;
	}
	public async Task<IActionResult> Index()
	{
		List<ProductDto> products = new();
		var response = await _productService.GetAllProductsAsycn();
		if(response != null && response.IsSuccess)
		{
			products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result)) ;
		}
		else
		{
			TempData["error"] = response?.Message;
		}
		return View(products);
	}

	public async Task<IActionResult> Details(int productId)
	{
		ProductDto product = new();
		var response = await _productService.GetProductAsync(productId);
		if (response != null && response.IsSuccess)
		{
			product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
		}
		else
		{
			TempData["error"] = response?.Message;
		}
		return View(product);
	}


	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Create(ProductDto product)
	{
		if (ModelState.IsValid)
		{
			var response = await _productService.CreateProductAsync(product);
			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
		}
		return View(product);
	}

	public async Task<IActionResult> Edit(int Id)
	{
		ProductDto product = new();
		var response = await _productService.GetProductAsync(Id);
		if(response != null && response.IsSuccess)
		{
			product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
		}
		else
		{
			TempData["error"] = response?.Message;
		}
		return View(product);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(ProductDto product)
	{
		if (ModelState.IsValid)
		{
			var response = await _productService.UpdateProductAsync(product);
			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
		}
		return View(product);
	}

	public async Task<IActionResult> Delete(int Id)
	{
		ProductDto product = new();
		var response = await _productService.GetProductAsync(Id);
		if(response != null && response.IsSuccess)
		{
			product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
			return View(product);
		}
		else
		{
			TempData["error"] = response?.Message;
		}
		return NotFound();
	}

	[HttpPost]
	public async Task<IActionResult> Delete(ProductDto product)
	{
		var response = await _productService.DeleteProductAsync(product.Id);
		if (response != null && response.IsSuccess)
		{
			TempData["success"] = "Coupon deleted successfully";
			return RedirectToAction(nameof(Index));
		}
		else
		{
			TempData["error"] = response?.Message;
		}
		return View(product);
	}
}
