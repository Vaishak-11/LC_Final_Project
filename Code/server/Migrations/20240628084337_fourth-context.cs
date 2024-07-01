using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommendationEngineServer.Migrations
{
    public partial class fourthcontext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "Cuisine",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FoodDiet",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpiceLevel",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cuisine",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "FoodDiet",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SpiceLevel",
                table: "Employees");

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
        }
    }
}
