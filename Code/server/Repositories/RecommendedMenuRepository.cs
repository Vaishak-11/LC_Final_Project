using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Models.Enums;
using RecommendationEngineServer.Repositories.Interfaces;

namespace RecommendationEngineServer.Repositories
{
    public class RecommendedMenuRepository : GenericRepository<RecommendedMenu>, IRecommendedMenuRepository
    {
        private readonly ServerDbContext _context;
        public RecommendedMenuRepository(ServerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RecommendedMenu> GetByItemId(int itemId, FoodCategory category, DateTime date)
        {
            return await _context.RecommendedMenus.FirstOrDefaultAsync(r => r.FoodItemId == itemId && r.Category == category && r.RecommendationDate.Date == date.Date);
        }

        public async Task<List<RecommendedMenu>> GetByItemIds(List<int> itemIds)
        {
            return await _context.RecommendedMenus
            .Where(r => (itemIds.Contains(r.FoodItemId)) && r.RecommendationDate ==DateTime.Now.AddDays(1))
            .ToListAsync();
        }

        public async Task<List<RecommendedMenu>> GetListByDate(DateTime? date = null)
        {
            if (date.HasValue)
            {
                return await _context.RecommendedMenus
                    .Where(r => r.RecommendationDate.Date == date.Value.Date)
                    .ToListAsync();
            }
            else
            {
                return await _context.RecommendedMenus.ToListAsync();
            }
        }
    }
}
