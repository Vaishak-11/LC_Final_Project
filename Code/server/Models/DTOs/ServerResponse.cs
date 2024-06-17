using RecommendationEngineServer.Models.Entities;

namespace RecommendationEngineServer.Models.DTOs
{
    public class ServerResponse
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
