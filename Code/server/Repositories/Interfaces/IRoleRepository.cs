using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Generic;

namespace RecommendationEngineServer.Repositories.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetRoleByName(string name);
    }
}
