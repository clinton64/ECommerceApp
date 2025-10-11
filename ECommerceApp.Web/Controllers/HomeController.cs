using ECommerceApp.Web.Models;
using ECommerceApp.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace ECommerceApp.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IProductService _productService;
		private readonly ICartService _cartService;

		public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
		{
			_logger = logger;
			_productService = productService;
			_cartService = cartService;
		}

		public async Task<IActionResult> Index()
		{
			var products = new List<ProductDto>();

			var response = await _productService.GetAllProductsAsycn();
			if(response != null && response.IsSuccess)
			{
				products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
				return View(products);
		}

		public async Task<IActionResult> ProductDetails(int productId)
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

		[HttpPost]
		public async Task<IActionResult> ProductDetails(ProductDto product)
		{
			var cart = new CartDto
			{
				CartHeader = new() { UserId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value }
			};

			var cartDetails = new CartDetailsDto()
			{
				Count = product.Count,
				ProductId = product.Id
			};

			cart.CartDetails = new List<CartDetailsDto>() { cartDetails };
			
			var response = await _cartService.UpsertCartAsync(cart);
			if (response != null && response.IsSuccess)
			{
				RedirectToAction(nameof(Index));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return View(product);
		}

		[HttpGet]
		public async Task<IActionResult> AddToCart()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddToCart(CartDto cart)
		{
			await _cartService.UpsertCartAsync(cart);	
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
