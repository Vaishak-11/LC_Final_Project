using AutoMapper;
using Castle.Core.Internal;
using Microsoft.Extensions.Logging;
using RecommendationEngineServer.Helpers;
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
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(IFeedbackRepository feedBackRepository, IFoodItemRepository foodItemRepository, IMapper mapper, IOrderItemRepository orderItemRepository, ILogger<FeedbackService> logger, INotificationService notificationService)
        {
            _feedBackRepository = feedBackRepository;
            _foodItemRepository = foodItemRepository;
            _mapper = mapper;
            _orderItemRepository = orderItemRepository;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<ServerResponse> AddFeedback(FeedbackDTO feedback)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                _logger.LogInformation($"userId: {feedback.UserId}, message: AddFeedback called., Date: {DateTime.Now}");

                if (feedback == null)
                {
                    _logger.LogInformation($"userId: {feedback.UserId}, message: Invalid feedback details., Date: {DateTime.Now}");
                    throw new ArgumentException("Invalid feedback details. Enter proper details.");
                }

                FoodItem item = await _foodItemRepository.GetByItemName(feedback.ItemName);

                if (item == null)
                {
                    _logger.LogInformation($"userId: {feedback.UserId}, message: This item name doesn't exist., Date: {DateTime.Now}");
                    throw new Exception("This item name doesn't exist.");
                }

                bool hasOrdered = await HasUserOrderedItem(feedback.UserId, item.FoodItemId);

                if (hasOrdered)
                {
                    Feedback newfeedback = _mapper.Map<FeedbackDTO, Feedback>(feedback);
                    newfeedback.FoodItemId = item.FoodItemId;
                    int feedbackId = await _feedBackRepository.Add(newfeedback);

                    response.Name = "AddFeedback";
                    response.Value = (feedbackId > 0) ? "Thank you for your feedback." : "Adding Feedback failed.";

                    _logger.LogInformation($"userId: {feedback.UserId}, message: Feedback Added, Date: {DateTime.Now}");
                }
                else
                {
                    response = ResponseHelper.CreateResponse("AddFeedback", "Sorry! You have not ordered this item. Please order this item to give feedback.");
                    _logger.LogInformation($"userId: {feedback.UserId}, message: tried to give feedback without prior orders., Date: {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
                _logger.LogError($"userId: {feedback.UserId}, message: {ex.Message}, Date: {DateTime.Now}");
            }

            return response;
        }

        public async Task<ServerResponse> GetFeedbacks(string itemName = null)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"message: GetFeedbacks called by userId: {UserData.UserId}, Date: {DateTime.Now}");

            try
            {
                FoodItem item = await _foodItemRepository.GetByItemName(itemName) ?? throw new Exception("This item name does not exist.");

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
                    _logger.LogInformation($"message: No feedbacks available for this item., Date: {DateTime.Now}");
                }

                response.Name = "Feedback List";
            }

            catch (Exception ex)
            {
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
                _logger.LogError($"message: {ex.Message}, Date: {DateTime.Now}");
            }

            return response;
        }

        public async Task<ServerResponse> GetDetailedFeedback(string itemName)
        {
            ServerResponse serverResponse = new ServerResponse();
            _logger.LogInformation($"Get detailed feedback method called by userId: {UserData.UserId}, DateTime: {DateTime.Now}");

            try
            {
                Expression<Func<Feedback, bool>> predicate = f => f.FoodItem.ItemName == itemName && f.Comment.ToLower().Contains("detailedfb") && f.Rating == null;
                List<Feedback> feedbacks = (await _feedBackRepository.GetList(predicate: predicate)).ToList();

                if (feedbacks.IsNullOrEmpty())
                {
                    _logger.LogInformation("No feedbacks are given.");
                    return ResponseHelper.CreateResponse("DetailedFeedback", "No feedbacks are given.");
                }


                List<string> feedbackList = feedbacks.Select(f => f.Comment).ToList();

                serverResponse = ResponseHelper.CreateResponse("DetailedFeedback", JsonSerializer.Serialize(feedbackList));
            }
            catch (Exception ex)
            {
                _logger.LogError($"message: {ex.Message}, DateTime: {DateTime.Now}");
                serverResponse = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return serverResponse;
        }

        public async Task<ServerResponse> NotifyEmployeesForFeedback(string itemName)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"message: NotifyEmployeesForFeedback called by userId: {UserData.UserId} for item: {itemName}, Date: {DateTime.Now}");

            try
            {
                Notification newNotification = new Notification
                {
                    Message = $"We are trying to improve your experience with the dish {itemName}. Please provide your feedback and help us.",
                    UserId = UserData.UserId,
                    IsDelivered = false
                };

                int notificationId = await _notificationService.AddNotification(newNotification);

                if(notificationId > 0)
                {
                    response = ResponseHelper.CreateResponse("NotifyEmployeesforFeedback", "added notification successfully.");
                }
                else
                {
                    response = ResponseHelper.CreateResponse("NotifyEmployeesforFeedback", "adding notification failed.");
                }
            }
            catch (Exception ex)
            {
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
                _logger.LogError($"message: {ex.Message}, Date: {DateTime.Now}");
            }

            return response;
        }

        private async Task<bool> HasUserOrderedItem(int userId, int foodItemId)
        {
            return await _orderItemRepository.OrderItemExists(oi => oi.Order.UserId == userId && oi.RecommendedMenu.FoodItemId == foodItemId);
        }
    }
}
