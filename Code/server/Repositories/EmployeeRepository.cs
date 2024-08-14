using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;

namespace RecommendationEngineServer.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly ServerDbContext _context;
        public EmployeeRepository(ServerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee> GetByEmployeeCode(string code)
        {
            return _context.Employees.Where(e => e.EmployeeCode == code).FirstOrDefault();
        }
    }
}
