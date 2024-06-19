using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;

namespace RecommendationEngineServer.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ServerDbContext _context;
        public OrderRepository(ServerDbContext context) : base(context)
        {
                _context = context;
        }

        public async Task<List<Order>> GetListByDate(DateTime? date = null)
        {
            if (date.HasValue)
            {
                return await _context.Orders
                    .Where(r => r.OrderDate.Date == date.Value.Date)
                    .ToListAsync();
            }
            else
            {
                return await _context.Orders.ToListAsync();
            }
        }
    }
}
