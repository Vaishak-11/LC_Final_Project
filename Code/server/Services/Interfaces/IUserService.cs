using RecommendationEngineServer.Models.DTOs;
namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServerResponse> Login(UserLoginDTO user);

        Task<ServerResponse> RegisterUser(UserLoginDTO user);
    }
}
