using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Models.Enums;
using RecommendationEngineServer.Models.DTOs;

namespace ServerUnitTests.testData
{
    public class RecommendedMenuTestData
    {
        public static List<RecommendedMenu> RecommendedMenuData()
        {
            return new List<RecommendedMenu>
            {
                new RecommendedMenu { MenuId=1, UserId=1, FoodItemId = 1, Category = FoodCategory.Breakfast, IsRecommended = false, RecommendationDate = DateTime.Now },
                new RecommendedMenu { MenuId=2, UserId=1, FoodItemId = 2, Category = FoodCategory.Lunch, IsRecommended = false, RecommendationDate = DateTime.Now },
            };
        }

        public static List<RecommendedMenuDTO> RecommendedMenuDTOs()
        {
            var recommendations = new List<RecommendedMenuDTO>
            {
                new RecommendedMenuDTO { ItemName = "Dosa", Category = FoodCategory.Breakfast, UserId = 1 },
                new RecommendedMenuDTO { ItemName = "Rice", Category = FoodCategory.Lunch, UserId = 1 }
            };

            return recommendations;
        }

        public static List<FoodItem> FoodItems()
        {
            return new List<FoodItem>
            {
                new FoodItem
                {
                    FoodItemId = 1,
                    ItemName = "Dosa",
                    Price = 25,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Breakfast,
                    IsAvailable = true
                },
                new FoodItem
                {
                    FoodItemId = 2,
                    ItemName = "Rice",
                    Price = 30,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Lunch,
                    IsAvailable = true
                },
                new FoodItem
                {
                    FoodItemId = 3,
                    ItemName = "Roti curry",
                    Price = 70,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Dinner,
                    IsAvailable = true
                }
            };
        }

        public static List<Employee> Employees()
        {
            return new List<Employee>
            {
                new Employee { EmployeeId = 1, EmployeeCode = "emp1234", UserId = 1, Cuisine = Cuisine.NorthIndian, FoodDiet = FoodDiet.Vegetarian, SpiceLevel= SpiceLevel.Medium },
                new Employee { EmployeeId = 2, EmployeeCode = "emp5678", UserId = 2, Cuisine = Cuisine.SouthIndian, FoodDiet = FoodDiet.NonVegetarian, SpiceLevel= SpiceLevel.High }
            };
        }
    }
}
