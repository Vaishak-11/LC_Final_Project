using RecommendationEngineServer.Models.DTOs;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IRequestHandlerService
    {
        Task<ServerResponse> ProcessRequest(string request);
    }
}
