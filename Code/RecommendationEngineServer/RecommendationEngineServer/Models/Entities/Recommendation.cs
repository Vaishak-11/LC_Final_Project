namespace RecommendationEngineServer.Models.Entities
{
    public class Recommendation
    {
        public int RecommendationId { get; set; }

        public int UserId { get; set; }

        public int MenuId { get; set; }

        public bool IsRecommended { get; set; }

        public DateTime RecommendationDate { get; set; }

        public User User { get; set; }

        public Menu Menu { get; set; }
    }
}
