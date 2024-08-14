using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; }

        public int UserId { get; set; }

        public FoodDiet FoodDiet { get; set; } = FoodDiet.NoPreference;

        public SpiceLevel SpiceLevel { get; set; } = SpiceLevel.Medium;

        public Cuisine Cuisine { get; set; } = Cuisine.NoPreference;

        public virtual User User { get; set; }
    }
}
