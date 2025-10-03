using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerceApp.Services.ProductService.Migrations
{
    /// <inheritdoc />
    public partial class AddProduct_ProductService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageLocalPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "ImageLocalPath", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Smart Phone", "IPhone 13 from Apple", "", "https://fdn2.gsmarena.com/vv/bigpic/apple-iphone-15.jpg", "IPhone 13", 999.99000000000001 },
                    { 2, "Smart Phone", "Samsung Galaxy S21 from Samsung", "", "https://fdn2.gsmarena.com/vv/bigpic/samsung-galaxy-s21-5g-r.jpg", "Samsung Galaxy S21", 899.99000000000001 },
                    { 3, "Smart Phone", "Google Pixel 6 from Google", "", "https://fdn2.gsmarena.com/vv/pics/google/google-pixel-6-1.jpg", "Google Pixel 6", 799.99000000000001 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
