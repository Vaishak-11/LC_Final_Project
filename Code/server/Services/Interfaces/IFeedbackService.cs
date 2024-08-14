using RecommendationEngineServer.Models.DTOs;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IFeedbackService 
    {
        Task<ServerResponse> AddFeedback(FeedbackDTO feedbackDTO);

        Task<ServerResponse> GetFeedbacks(string itemName=null);

        Task<ServerResponse> GetDetailedFeedback(string itemName);

        Task<ServerResponse> NotifyEmployeesForFeedback(string itemName);

    }
}
