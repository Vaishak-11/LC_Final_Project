using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;

namespace RecommendationEngineServer.Repositories
{
    public class FoodItemRepository : GenericRepository<FoodItem>, IFoodItemRepository
    {
        private readonly ServerDbContext _context;
        public FoodItemRepository(ServerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<FoodItem> GetByItemName(string itemName)
        {
            return _context.FoodItems.Where(m => m.ItemName.ToLower().Trim() == itemName.ToLower().Trim()).FirstOrDefault();
        }

        public async Task<List<FoodItem>> GetByItemNames(List<string> itemNames)
        {
            return _context.FoodItems.Where(m => itemNames.Contains(m.ItemName.ToLower().Trim()) && m.IsAvailable == true).ToList();
        }

        public override async Task Update(FoodItem entity)
        {
            try
            {
                var existingEntity = await _context.FoodItems.FindAsync(entity.FoodItemId);

                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentException("Food item not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating food item", ex);
            }
        }

        public async Task<bool> HasAssociatedOrders(int foodItemId)
        {
            return _context.OrderItems.Any(m => m.RecommendedMenu.FoodItemId == foodItemId);
        }
    }
}
