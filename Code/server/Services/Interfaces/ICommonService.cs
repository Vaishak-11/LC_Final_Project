using RecommendationEngineServer.Models.DTOs;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface ICommonService
    {
        ServerResponse CreateResponse(string name, Object value);
    }
}
