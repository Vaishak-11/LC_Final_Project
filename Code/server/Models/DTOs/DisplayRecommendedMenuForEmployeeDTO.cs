using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.DTOs
{
    public class DisplayRecommendedMenuForEmployeeDTO
    {
        public string ItemName { get; set; }

        public decimal Price { get; set; }

        public FoodCategory FoodCategory { get; set; }

        public double Rating { get; set; }

        public string OverallRating { get; set; }

        public string RecommendationReason { get; set; }
    }
}
