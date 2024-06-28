using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.Entities
{
    public class Recommendation
    {
        public int RecommendationId { get; set; }

        public int UserId { get; set; }

        public int MenuId { get; set; }

        public FoodCategory Category { get; set; }

        public bool IsRecommended { get; set; } = false;

        public DateTime RecommendationDate { get; set; }

        public virtual User User { get; set; }

        public virtual FoodItem Menu { get; set; }
    }
}
