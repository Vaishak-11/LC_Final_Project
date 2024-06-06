namespace RecommendationEngineServer.Models.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public string? Message { get; set; }

        public bool IsDelivered { get; set; }

        public User User { get; set; }
    }
}
