using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using System.Linq.Expressions;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IFeedbackService 
    {
        Task<ServerResponse> AddFeedback(FeedbackDTO feedbackDTO);
        Task<ServerResponse> GetFeedbacks(string itemName=null);
    }
}
