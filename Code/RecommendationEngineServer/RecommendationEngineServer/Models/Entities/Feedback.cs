namespace RecommendationEngineServer.Models.Entities
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public int UserId { get; set; }

        public int MenuId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime FeedbackDate { get; set; }

        public User User { get; set; }

        public Menu Menu { get; set; }
    }
}
