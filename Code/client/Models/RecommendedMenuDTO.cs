using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngineClient.Models
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
