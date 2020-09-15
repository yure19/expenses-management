using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpensesMgmtWeb.Migrations
{
    public partial class AddedConfirmedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "PurchasedProduct",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Purchase",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "PurchasedProduct");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Purchase");
        }
    }
}
