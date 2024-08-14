using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.DTOs
{
    public class RecommendedMenuDTO
    {
        public int UserId { get; set; }

        public string ItemName { get; set; }

        public bool IsRecommended { get; set; }

        public FoodCategory Category { get; set; }

        public DateTime RecommendationDate { get; set; }

        public string OldItemName { get; set; }

        public FoodCategory OldCategory { get; set; }
    }
}
