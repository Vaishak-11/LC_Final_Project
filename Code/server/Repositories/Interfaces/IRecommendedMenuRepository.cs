using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Models.Enums;
using RecommendationEngineServer.Repositories.Generic;
namespace RecommendationEngineServer.Repositories.Interfaces
{ 
    public interface IRecommendedMenuRepository : IGenericRepository<RecommendedMenu>
    {
        Task<RecommendedMenu> GetByItemId(int itemId, FoodCategory category, DateTime date);

        Task<List<RecommendedMenu>> GetListByDate(DateTime? date = null);

        Task<List<RecommendedMenu>> GetByItemIds(List<int> itemIds);
    }
}
