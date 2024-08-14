using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.DTOs
{
    public class EmployeeProfileDTO
    {
        public int UserId { get; set; }

        public Cuisine Cuisine { get; set; }

        public SpiceLevel SpiceLevel { get; set; }

        public FoodDiet FoodDiet { get; set; }
    }
}
