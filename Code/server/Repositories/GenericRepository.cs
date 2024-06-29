using Microsoft.EntityFrameworkCore;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Repositories.Generic;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<T>> GetList(Expression<Func<T,bool>> predicate = null)
        {
            var query = _table.AsQueryable();

            if(predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _table.FindAsync(id);
        }

        public async Task<int> Add(T entity)
        {
            await _table.AddAsync(entity);
            await _context.SaveChangesAsync();
            return GetPrimaryId(entity);
        }

        public async Task<int> AddRange(IEnumerable<T> entities)
        {
            await _table.AddRangeAsync(entities);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task Update(T entity)
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

        private static int GetPrimaryId(T entity)
        {
            string idPropertyName = entity.GetType().Name + "Id";
            var idProperty = entity.GetType().GetProperty(idPropertyName);

            if (idProperty != null)
            {
                return (int)idProperty.GetValue(entity);
            }

            return 0;
        }
    }
}
