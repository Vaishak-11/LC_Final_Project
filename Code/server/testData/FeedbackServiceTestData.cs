using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUnitTests.testData
{
    public class FeedbackServiceTestData
    {
        public static List<Feedback> Feedbacks()
        {
            return new List<Feedback>
            {
                new Feedback
                {
                    FeedbackId = 1,
                    UserId = 1,
                    FoodItemId = 1,
                    Rating = 4,
                    Comment = "Good"
                },
                new Feedback
                {
                    FeedbackId = 2,
                    UserId = 2,
                    FoodItemId = 1,
                    Rating = 3,
                    Comment = "1.too much roasted 2. Authentic south-indian taste 3. apply less oil"
                    
                },
                new Feedback
                {
                    FeedbackId = 3,
                    UserId = 3,
                    FoodItemId = 2,
                    Rating = 5,
                    Comment = "Average"
                }
            };
        }

        public static List<FeedbackDTO> FeedbackDTOs()
        {
            return new List<FeedbackDTO>
            {
                new FeedbackDTO
                {
                    UserId = 1,
                    ItemName = "Burger",
                    Rating = 4,
                    Comment = "Good"
                },
                new FeedbackDTO
                {
                    UserId = 2,
                    ItemName = "Pizza",
                    Rating = 3,
                    Comment = "Average"
                },
                new FeedbackDTO
                {
                    UserId = 3,
                    ItemName = "Pasta",
                    Rating = 5,
                    Comment = "Excellent"
                }
            };
        }

        public static List<DisplayFeedbackDTO> DisplayFeedbackDTOs()
        {
            return new List<DisplayFeedbackDTO>
            {
                new DisplayFeedbackDTO
                {
                    Rating = "4",
                    Comment = "Good",
                    FeedbackDate = DateTime.Now 
                },
                new DisplayFeedbackDTO
                {

                    Rating = "3",
                    Comment = "Average",
                    FeedbackDate = DateTime.Now
                },
            };
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
    }
}
