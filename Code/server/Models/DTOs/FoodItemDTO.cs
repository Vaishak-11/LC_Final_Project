﻿using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.DTOs
{
    public class FoodItemDTO
    {
        public int FoodItemId { get; set; }

        public string ItemName { get; set; }

        public decimal Price { get; set; }

        public FoodCategory FoodCategory { get; set; }

        public bool IsAvailable { get; set; }
    }
}
