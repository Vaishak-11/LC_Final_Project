using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.Entities
{
    public class Menu
    {
        public int MenuId { get; set; }

        public string ItemName { get; set; }

        public decimal Price { get; set; }

        public FoodCategory FoodCategory { get; set; }

        public bool IsAvailable { get; set; }
    }
}
