using Microsoft.EntityFrameworkCore.Migrations;

namespace GeniiApp.Migrations
{
    public partial class AddedQuantitySoldRow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantitySold",
                table: "ProductInvoices",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantitySold",
                table: "ProductInvoices");
        }
    }
}
