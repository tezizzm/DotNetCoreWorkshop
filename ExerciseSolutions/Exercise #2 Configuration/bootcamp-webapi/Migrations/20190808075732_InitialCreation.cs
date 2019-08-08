using Microsoft.EntityFrameworkCore.Migrations;

namespace bootcamp_webapi.Migrations
{
    public partial class InitialCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Inventory = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Inventory", "Name" },
                values: new object[] { 1L, "Books", 5, "The Ultimate Guide To Budget Travel" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Inventory", "Name" },
                values: new object[] { 2L, "Sports", 4, "Upper Deck Baseball Set" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Inventory", "Name" },
                values: new object[] { 3L, "Groceries", 2, "Gatorade" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Inventory", "Name" },
                values: new object[] { 4L, "Electronics", 50, "Google Pixel 3" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Inventory", "Name" },
                values: new object[] { 5L, "Home and Garden", 20, "Kitchenette Stand Mixer" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
