using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Generic;

namespace RecommendationEngineServer.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee> GetByEmployeeCode(string code);
    }
}
