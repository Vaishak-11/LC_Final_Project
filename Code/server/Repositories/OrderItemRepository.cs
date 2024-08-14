using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using System.Linq.Expressions;

namespace RecommendationEngineServer.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        private readonly ServerDbContext _context;
        public OrderItemRepository(ServerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> OrderItemExists(Expression<Func<OrderItem, bool>> predicate)
        {
            return await _context.OrderItems.AnyAsync(predicate);
        }
    }
}
