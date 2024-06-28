using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Services.Interfaces;

namespace RecommendationEngineServer.Services
{
    public class CommonService : ICommonService
    {
        public ServerResponse CreateResponse(string name, Object value)
        {
            return new ServerResponse
            {
                Name = name,
                Value = value
            };
        }
    }
}
