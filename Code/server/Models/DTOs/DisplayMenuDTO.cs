using RecommendationEngineServer.Models.Enums;

namespace RecommendationEngineServer.Models.DTOs
{
    public class DisplayMenuDTO
    {
        public string ItemName { get; set; }

        public decimal Price { get; set; }  

        public double Rating { get; set; }

        public string Sentiment { get; set; }

        public List<string> Comments { get; set; }
    }
}
