using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Generic;
using System.Linq.Expressions;

namespace RecommendationEngineServer.Repositories.Interfaces
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task<bool> OrderItemExists(Expression<Func<OrderItem, bool>> predicate);
    }
}
