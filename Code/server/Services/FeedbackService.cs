using AutoMapper;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

namespace RecommendationEngineServer.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedBackRepository;
        private readonly IFoodItemRepository _foodItemRepository;
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository feedBackRepository, IFoodItemRepository foodItemRepository, IMapper mapper)
        {
            _feedBackRepository = feedBackRepository;
            _foodItemRepository = foodItemRepository;
            _mapper = mapper;
        }

        public async Task<ServerResponse> AddFeedback(FeedbackDTO feedback)
        {
            ServerResponse response = new ServerResponse();

            if (feedback != null)
            {
                FoodItem item = await _foodItemRepository.GetByItemName(feedback.ItemName);
                
                if (item == null)
                {
                    response.Name = "Error";
                    response.Value = "This itemName doesnt exist";

                    return response;
                }

                Feedback newfeedback = _mapper.Map<FeedbackDTO, Feedback>(feedback);
                newfeedback.FoodItemId = item.FoodItemId;
                int feedbackId = await _feedBackRepository.Add(newfeedback);

                response.Name = "AddFeedback";
                response.Value = (feedbackId > 0) ? "Thank you for you feedback." : "Adding Feedback failed.";
            }
            else
            {
                response.Name = "Error";
                response.Value = "Invalid feedback details.Enter proper details";
            }

            return response;
        }

        public async Task<ServerResponse> GetFeedbacks(string itemName = null)  
        {
            ServerResponse response = new ServerResponse();
  
            FoodItem item = await _foodItemRepository.GetByItemName(itemName);

            if (item == null)
            {
                response.Name = "Error";
                response.Value = "This itemName does not exist exists";

                return response;
            }

            Expression<Func<Feedback, bool>> predicate = f => f.FoodItem.FoodItemId == item.FoodItemId;
            List<Feedback> feedbackList = (await _feedBackRepository.GetList(predicate: predicate)).ToList();

            if (feedbackList.Any())
            {
                List<DisplayFeedbackDTO> feedbackDtoList = _mapper.Map<List<DisplayFeedbackDTO>>(feedbackList);
                response.Value = JsonSerializer.Serialize(feedbackDtoList);
            }
            else
            {
                response.Value = "No feedbacks available for this item.";
            }

            response.Name = "Feedback List";

            return response;
        }
    }
}
