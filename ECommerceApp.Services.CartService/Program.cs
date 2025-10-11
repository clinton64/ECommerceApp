using ECommerceApp.Services.CartService.Data;
using ECommerceApp.Services.CartService.Service;
using ECommerceApp.Services.CartService.Service.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ECommerceApp.Services.CartService.Service.HttpClientHandler>();
builder.Services.AddHttpClient("Product",
	u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrl:ProductAPI"])).AddHttpMessageHandler<ECommerceApp.Services.CartService.Service.HttpClientHandler>();

builder.Services.AddHttpClient("Coupon",
	u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrl:CouponAPI"])).AddHttpMessageHandler<ECommerceApp.Services.CartService.Service.HttpClientHandler>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
	securityScheme: new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference= new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id=JwtBearerDefaults.AuthenticationScheme
				}
			}, new string[]{}
		}
	});
});
builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
	x.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidateAudience = true,
		ValidAudience = builder.Configuration["Jwt:Audience"]
	};
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();
