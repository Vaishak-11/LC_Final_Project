using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories;
using RecommendationEngineServer.Context;
using RecommendationEngineServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserRepository :  GenericRepository<User>, IUserRepository
{
    private readonly ServerDbContext _context;

    public UserRepository(ServerDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetUserByName(string name)
    {
        return _context.Users.Where(u => u.UserName.ToLower().Trim() == name.ToLower().Trim()).FirstOrDefault();
    }
}
