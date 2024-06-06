using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Generic;

namespace RecommendationEngineServer.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByName(string name);
    }
}
