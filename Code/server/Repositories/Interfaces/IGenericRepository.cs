namespace RecommendationEngineServer.Repositories.Generic
{
    public interface IGenericRepository <T> where T : class
    {
        Task<IEnumerable<T>> GetList();
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}
