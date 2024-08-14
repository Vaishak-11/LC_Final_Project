using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommendationEngineServer.Migrations
{
    public partial class thirdcontext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_RecommendedMenus_MenuId",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_RecommendedMenus_MenuId",
                table: "OrderItems",
                column: "MenuId",
                principalTable: "RecommendedMenus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_RecommendedMenus_MenuId",
                table: "OrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_RecommendedMenus_MenuId",
                table: "OrderItems",
                column: "MenuId",
                principalTable: "RecommendedMenus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
