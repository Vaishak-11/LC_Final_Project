using RecommendationEngineServer.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RecommendationEngineServer.Models.Entities
{
    public class RecommendedMenu
    {
        [Key]
        public int MenuId { get; set; }

        public int UserId { get; set; }

        public int FoodItemId { get; set; }

        public FoodCategory Category { get; set; }

        public bool IsRecommended { get; set; } = false;

        public DateTime RecommendationDate { get; set; }

        public virtual User User { get; set; }

        public virtual FoodItem FoodItem { get; set; }
    }
}
