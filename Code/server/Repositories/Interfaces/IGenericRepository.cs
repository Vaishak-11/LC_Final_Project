using System.Linq.Expressions;

namespace RecommendationEngineServer.Repositories.Generic
{
    public interface IGenericRepository <T> where T : class
    {
        Task<IEnumerable<T>> GetList(Expression<Func<T, bool>> predicate = null);
        Task<T> GetById(int id);
        Task<int> Add(T entity);
        Task<int> AddRange(IEnumerable<T> entities);
        Task Update(T entity);
        Task Delete(int id);
    }
}
