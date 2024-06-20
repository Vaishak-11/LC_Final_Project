using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUnitTests.testData
{
    public class FoodItemTestData
    {
        public static List<FoodItem> FoodItems()
        {
            return new List<FoodItem>
            {
                new FoodItem
                {
                    FoodItemId = 1,
                    ItemName = "Dosa",
                    Price = 25,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Breakfast
                },
                new FoodItem
                {
                    FoodItemId = 2,
                    ItemName = "Rice",
                    Price = 30,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Lunch
                },
                new FoodItem
                {
                    FoodItemId = 3,
                    ItemName = "Roti curry",
                    Price = 70,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Dinner
                }
            };
        }

        public static List<FoodItemDTO> FoodItemDTO()
        {
            return new List<FoodItemDTO>
            {
                new FoodItemDTO
                {
                    ItemName = "Dosa",
                    Price = 25,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Breakfast
                },
                new FoodItemDTO
                {
                    ItemName = "Rice",
                    Price = 30,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Lunch
                },
                new FoodItemDTO
                {
                    ItemName = "Roti curry",
                    Price = 70,
                    FoodCategory = RecommendationEngineServer.Models.Enums.FoodCategory.Dinner
                }
            };
        }

        public static List<Notification> Notifications()
        {
            return new List<Notification>
            {
                new Notification
                {
                    NotificationId = 1,
                    Message = "Food item added successfully",
                    UserId = 1,
                    IsDelivered = false
                },
                new Notification
                {
                    NotificationId = 2,
                    Message = "Food item updated successfully",
                    UserId = 1,
                    IsDelivered = false
                }
            };
        }

        public static List<Feedback> Feedbacks()
        {
            return new List<Feedback>
            {
                new Feedback
                {
                    FeedbackId = 1,
                    FoodItemId = 1,
                    UserId = 1,
                    Rating = 4,
                    Comment = "Good"
                },
                new Feedback
                {
                    FeedbackId = 2,
                    FoodItemId = 2,
                    UserId = 1,
                    Rating = 3,
                    Comment = "Bad"
                }
            };
        }
    }
}
