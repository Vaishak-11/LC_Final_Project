using RecommendationEngineServer.Models.Entities;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> Login(string name, string password, Role role);
    }
}
