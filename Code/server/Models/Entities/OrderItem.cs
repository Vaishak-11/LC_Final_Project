namespace RecommendationEngineServer.Models.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int MenuId { get; set; }

        public virtual Order Order { get; set; }

        public virtual RecommendedMenu RecommendedMenu { get; set; }
    }
}
