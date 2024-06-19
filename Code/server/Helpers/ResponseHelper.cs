using RecommendationEngineServer.Models.DTOs;

namespace RecommendationEngineServer.Helpers
{
    public static class ResponseHelper
    {
        public static ServerResponse CreateResponse(string name, Object value)
        {
            return new ServerResponse
            {
                Name = name,
                Value = value
            };
        }
    }
}
