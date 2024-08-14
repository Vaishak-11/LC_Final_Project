using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Generic;

namespace RecommendationEngineServer.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<List<Order>> GetListByDate(DateTime? date = null);
    }
}
