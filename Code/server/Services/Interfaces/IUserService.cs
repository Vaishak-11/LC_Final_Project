using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServerResponse> Login(string name, string password, Role role);
    }
}
