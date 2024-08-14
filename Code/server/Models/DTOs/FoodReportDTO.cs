namespace RecommendationEngineServer.Models.DTOs
{
    public class FoodReportDTO
    {
        public string ItemName { get; set; }
        public List<string> Comments { get; set; }
        public double AverageRating { get; set; }
    }
}
