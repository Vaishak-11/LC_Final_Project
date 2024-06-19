using RecommendationEngineServer.Models.DTOs;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IRecommendedMenuService
    {
        Task<ServerResponse> AddRecommendedMenu(List<RecommendedMenuDTO> recommendations);

        Task<ServerResponse> GetRecommendedMenu(DateTime? date = null);

        Task<ServerResponse> UpdateRecommendedMenu(RecommendedMenuDTO recommendedMenu);
    }
}
