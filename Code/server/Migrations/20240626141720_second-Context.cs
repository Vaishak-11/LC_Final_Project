using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommendationEngineServer.Migrations
{
    public partial class secondContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cuisine",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FoodDiet",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpiceLevel",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Cuisine",
                table: "FoodItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FoodDiet",
                table: "FoodItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpiceLevel",
                table: "FoodItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Feedbacks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cuisine",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FoodDiet",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpiceLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Cuisine",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "FoodDiet",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "SpiceLevel",
                table: "FoodItems");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
