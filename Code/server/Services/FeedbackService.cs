﻿using AutoMapper;
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
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository feedBackRepository, IFoodItemRepository foodItemRepository, IMapper mapper, IOrderItemRepository orderItemRepository)
        {
            _feedBackRepository = feedBackRepository;
            _foodItemRepository = foodItemRepository;
            _mapper = mapper;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<ServerResponse> AddFeedback(FeedbackDTO feedback)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                if (feedback == null)
                {
                    throw new ArgumentException("Invalid feedback details. Enter proper details.");
                }

                FoodItem item = await _foodItemRepository.GetByItemName(feedback.ItemName) ?? throw new Exception("This item name doesn't exist."); ;

                bool hasOrdered = await HasUserOrderedItem(feedback.UserId, item.FoodItemId);

                if (hasOrdered)
                {
                    Feedback newfeedback = _mapper.Map<FeedbackDTO, Feedback>(feedback);
                    newfeedback.FoodItemId = item.FoodItemId;
                    int feedbackId = await _feedBackRepository.Add(newfeedback);

                    response.Name = "AddFeedback";
                    response.Value = (feedbackId > 0) ? "Thank you for you feedback." : "Adding Feedback failed.";
                }
                else
                {
                    response = ResponseHelper.CreateResponse("AddFeedback", "Sorry! You have not ordered this item. Please order this item to give feedback.");
                }
            }
            catch (Exception ex)
            {
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> GetFeedbacks(string itemName = null)  
        {
            ServerResponse response = new ServerResponse();

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
                }

                response.Name = "Feedback List";
            }

            catch (Exception ex)
            {
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        private async Task<bool> HasUserOrderedItem(int userId, int foodItemId)
        {
            return await _orderItemRepository.OrderItemExists(oi =>oi.Order.UserId == userId && oi.RecommendedMenu.FoodItemId == foodItemId);
        }
    }
}
