using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpensesMgmtWeb.Migrations
{
    public partial class StoreIdPurchCanBeNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Store_StoreId",
                table: "Purchase");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Purchase",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Store_StoreId",
                table: "Purchase",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Store_StoreId",
                table: "Purchase");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Purchase",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Store_StoreId",
                table: "Purchase",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
