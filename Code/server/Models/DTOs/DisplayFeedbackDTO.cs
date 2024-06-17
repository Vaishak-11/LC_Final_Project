namespace RecommendationEngineServer.Models.DTOs
{
    public class DisplayFeedbackDTO
    {
        public string Rating { get; set; }

        public DateTime FeedbackDate { get; set; }

        public string Comment { get; set; }
    }
}
