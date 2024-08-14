using RecommendationEngineServer.Context;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;

namespace RecommendationEngineServer.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly ServerDbContext _context;
        public RoleRepository(ServerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Role> GetRoleByName(string name)
        {
            return _context.Roles.Where(r => r.RoleName.ToLower().Trim() == name.ToLower().Trim()).FirstOrDefault();
        }
    }
}
