using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Repositories.Generic;

namespace RecommendationEngineServer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ServerDbContext _context;
        private readonly DbSet<T> _table;

        public GenericRepository(ServerDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetList()
        {
            return await _table.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _table.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _table.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _table.FindAsync(id);
            _table.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
