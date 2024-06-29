namespace RecommendationEngineClient.Models
{
    public class EmployeeProfileDTO
    {
        public int UserId { get; set; } 

        public Cuisine Cuisine { get; set; }

        public SpiceLevel SpiceLevel { get; set; }

        public FoodDiet FoodDiet { get; set; }
    }

    public enum Cuisine
    {
        NoPreference = 0,
        NorthIndian = 1,
        SouthIndian = 2,
        Chinese = 3,
        Other = 4
    }

    public enum SpiceLevel
    {
        Low = -1,
        Medium = 0,
        High = 1
    }

    public enum FoodDiet
    {
        Vegetarian = 1,
        NonVegetarian = 2,
        Eggetarian = 3,
        NoPreference = 0
    }
}
