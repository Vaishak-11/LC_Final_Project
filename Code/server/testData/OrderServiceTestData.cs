using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUnitTests.testData
{
    public class OrderServiceTestData
    {
        public static List<Order> Orders()
        {
            return new List<Order>()
            {
                new Order
                {
                    OrderId = 1,
                    UserId = 1,
                    OrderDate = DateTime.Now,
                    User = new User
                    {
                        UserId = 1,
                        UserName = "User1",
                    }
                },
                new Order
                {
                    OrderId = 2,
                    UserId = 2,
                    OrderDate = DateTime.Now,
                    User = new User
                    {
                        UserId = 2,
                        UserName = "User2",
                    }
                },
            };
        }

        public static List<OrderDTO> OrderDTOs()
        {
            return new List<OrderDTO>()
            {
                new OrderDTO
                {
                    UserId = 1,
                    ItemNames = new List<string> { "Dosa", "Rice" }
                },
                new OrderDTO
                {
                    UserId = 2,
                    ItemNames = new List<string> { "Roti curry" }
                },
            };
        }

        public static List<FoodItem> FoodItems()
        {
            return new List<FoodItem>()
            {
                new FoodItem
                {
                    FoodItemId = 1,
                    ItemName = "Dosa",
                    Price = 50,
                },
                new FoodItem
                {
                    FoodItemId = 2,
                    ItemName = "Rice",
                    Price = 30,
                },
                new FoodItem
                {
                    FoodItemId = 3,
                    ItemName = "Roti curry",
                    Price = 40,
                },
            };
        }

        public static List<RecommendedMenu> RecommendedMenus()
        {
            return new List<RecommendedMenu>()
            {
                new RecommendedMenu
                {
                    MenuId = 1,
                    FoodItemId = 1,
                },
                new RecommendedMenu
                {
                    MenuId = 2,
                    FoodItemId = 2,
                },
            };
        }

        public static List<OrderItem> OrderItems()
        {
            return new List<OrderItem>()
            {
                new OrderItem
                {
                    OrderItemId = 1,
                    OrderId = 1,
                    MenuId = 1,
                    RecommendedMenu = new RecommendedMenu
                    {
                        MenuId = 1,
                        FoodItemId = 1,
                    }
                },
                new OrderItem
                {
                    OrderItemId = 2,
                    OrderId = 1,
                    MenuId = 2,
                    RecommendedMenu = new RecommendedMenu
                    {
                        MenuId = 2,
                        FoodItemId = 2,
                    }
                },
            };
        }
    }
}
