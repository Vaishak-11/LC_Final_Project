using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Generic;

namespace RecommendationEngineServer.Repositories.Interfaces
{
    public interface IFoodItemRepository : IGenericRepository<FoodItem>
    {
        Task<FoodItem> GetByItemName(string name);

        Task<List<FoodItem>> GetByItemNames(List<string> itemNames);

        Task Update(FoodItem entity);
    }
}
