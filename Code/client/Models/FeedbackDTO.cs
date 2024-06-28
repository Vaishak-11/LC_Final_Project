namespace RecommendationEngineClient.Models
{
    public class FeedbackDTO
    {
        public int UserId { get; set; }

        public string? ItemName { get; set; }

        public int? Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime FeedbackDate { get; set; }
    }
}
