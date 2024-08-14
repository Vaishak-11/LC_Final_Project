using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.Entities
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }

        public string ItemName { get; set; }

        public decimal Price { get; set; }

        public FoodCategory FoodCategory { get; set; }

        public FoodDiet FoodDiet { get; set; } = FoodDiet.NoPreference;

        public SpiceLevel SpiceLevel { get; set; } = SpiceLevel.Medium;

        public Cuisine Cuisine { get; set; } = Cuisine.NoPreference;

        public bool IsAvailable { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; }

        public virtual ICollection<RecommendedMenu> RecommendedMenus { get; set; }
    }
}
