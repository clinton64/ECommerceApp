using ECommerceApp.Services.CartService.Data;
using ECommerceApp.Services.CartService.Service;
using ECommerceApp.Services.CartService.Service.IService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpClient("Product",
	u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrl.ProductAPI"])).AddHttpMessageHandler<ECommerceApp.Services.CartService.Service.HttpClientHandler>();

builder.Services.AddHttpClient("Coupon",
	u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrl.CouponAPI"])).AddHttpMessageHandler<ECommerceApp.Services.CartService.Service.HttpClientHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
