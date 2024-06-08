namespace RecommendationEngineServer.Models.Entities
{
    public class Recommendation
    {
        public int RecommendationId { get; set; }

        public int UserId { get; set; }

        public int MenuId { get; set; }

        public bool IsRecommended { get; set; }

        public DateTime RecommendationDate { get; set; }

        public virtual User User { get; set; }

        public virtual Menu Menu { get; set; }
    }
}
