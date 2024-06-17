using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngineClient.Models
{
    public class FoodItemDTO
    {
        public int FoodItemId { get; set; }

        public string ItemName { get; set; }

        public decimal Price { get; set; }

        public FoodCategory FoodCategory { get; set; }

        public bool IsAvailable { get; set; }
    }

    public enum FoodCategory
    {
        Breakfast=1,
        Lunch=2,
        Dinner=3,
        Beverages=4,
        None
    }
}
