using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
namespace RecommendationEngineServer.Services.Interfaces
{
    public interface INotificationService
    {
        Task<int> AddNotification(Notification notification);

        Task<ServerResponse> GetNotifications(int UserId = 0);

        Task<ServerResponse> UpdateNotificationStatus(int notificationId);

    }
}
