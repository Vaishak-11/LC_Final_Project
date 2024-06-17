namespace RecommendationEngineServer.Models.Entities
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public int UserId { get; set; }

        public int FoodItemId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime FeedbackDate { get; set; }

        public virtual User User { get; set; }

        public virtual FoodItem FoodItem { get; set; }
    }
}
