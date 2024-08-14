namespace RecommendationEngineServer.Models.DTOs
{
    public class OrderDTO
    {
        public int UserId { get; set; }

        public List<string> ItemNames { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
