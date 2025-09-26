using ECommerceApp.Services.AuthService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<AppDBContext>()
	.AddDefaultTokenProviders();
builder.Services.AddDbContext<AppDBContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IDBInitializer, DBInitializer>();
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

SeedDB();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Initialize database
void SeedDB()
{
	using (var scope = app.Services.CreateScope())
	{
		var dBInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
		dBInitializer.initialize();
	}
}
