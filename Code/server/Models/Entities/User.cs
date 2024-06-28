using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int RoleId { get; set; }

        public FoodDiet FoodDiet { get; set; } = FoodDiet.NoPreference;

        public SpiceLevel SpiceLevel { get; set; } = SpiceLevel.Medium;

        public Cuisine Cuisine { get; set; } = Cuisine.NoPreference;

        public virtual Role Role { get; set; }
    }
}
