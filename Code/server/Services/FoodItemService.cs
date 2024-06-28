using AutoMapper;
using Castle.Core.Internal;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecommendationEngineServer.Helpers;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;
using System.Text.Json;

namespace RecommendationEngineServer.Services
{
    public class FoodItemService : IFoodItemService
    {
        private readonly IFoodItemRepository _foodItemRepository;
        private readonly INotificationService _notificationService;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FoodItemService> _logger;

        public FoodItemService(IFoodItemRepository foodItemRepository, IMapper mapper, INotificationService notificationService, IFeedbackRepository feedbackRepository, ILogger<FoodItemService> logger)
        {
            _foodItemRepository = foodItemRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _feedbackRepository = feedbackRepository;
            _logger = logger;
        }

        public async Task<ServerResponse> Add(FoodItemDTO item)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Add food item method called by userId: {UserData.UserId}, DateTime: {DateTime.Now}");
            try
            {
                if (item == null)
                {
                    _logger.LogError("Invalid item.");
                    throw new ArgumentException("Invalid item. Enter proper details.");
                }

                var existingItem = await _foodItemRepository.GetByItemName(item.ItemName);

                if (existingItem != null)
                {
                    _logger.LogError("Item already exists.");
                    throw new ArgumentException("This item name already exists.");
                }

                var newItem = _mapper.Map<FoodItem>(item);
                var itemId = await _foodItemRepository.Add(newItem);

                string message = itemId > 0 ? "Item added successfully." : "Adding item failed.";
                response = ResponseHelper.CreateResponse("AddItem", message);
                _logger.LogInformation($"{message} , DateTime: {DateTime.Now}");

                if (itemId > 0)
                {
                    await _notificationService.AddNotification(new Notification
                    {
                        UserId = UserData.UserId,
                        Message = $"New item {item.ItemName} added to the menu list.",
                        IsDelivered = false
                    });
                }
            }
            catch (Exception ex)
            {
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
                _logger.LogError($"message: {ex.Message}, DateTime: {DateTime.Now}");
            }

            return response;
        }

        public async Task<ServerResponse> GetList()
        {
            var response = new ServerResponse();
            _logger.LogInformation($"message: get feedbacks method called by userId: {UserData.UserId}, DateTime: {DateTime.Now}");

            try
            {
                List<FoodItem> itemList = (await _foodItemRepository.GetList(ft=> ft.IsAvailable == true)).ToList();

                if (!itemList.Any())
                {
                    _logger.LogError($"No items found. DateTime: {DateTime.Now}");
                    throw new Exception("No items found.");
                }

                var itemIds = itemList.Select(i => i.FoodItemId).ToList();
                var feedbackList = (await _feedbackRepository.GetList(f => itemIds.Contains(f.FoodItemId))).ToList();

                var itemTasks = itemList.Select(async foodItem =>
                {
                    var feedbacks = feedbackList.Where(f => f.FoodItemId == foodItem.FoodItemId).ToList();
                    var averageRating = feedbacks.Any() ? Math.Round((double)feedbacks.Average(f => f.Rating), 2) : 0;
                    var comments = feedbacks.Any() ? feedbacks.Select(f => f.Comment).ToList() : new List<string>();
                    var overallRating = await SentimentAnlysisHelper.AnalyzeSentiments(comments, averageRating);

                    return new DisplayMenuDTO
                    {
                        ItemName = foodItem.ItemName,
                        Price = foodItem.Price,
                        FoodCategory = foodItem.FoodCategory,
                        Rating = averageRating,
                        OverallRating = overallRating,
                    };
                }).ToList();

                var items = (await Task.WhenAll(itemTasks)).ToList();
                items = items.OrderBy(d => d.FoodCategory).ThenByDescending(d => d.OverallRating).ToList();

                string message = !items.IsNullOrEmpty() ? JsonSerializer.Serialize(items) : "Not food items available to display";
                response = ResponseHelper.CreateResponse("ItemList", message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"message: {ex.Message}, DateTime: {DateTime.Now}");
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> Update(string oldName, FoodItemDTO itemDto, string avilability)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Update food item method called by userId: {UserData.UserId}, DateTime: {DateTime.Now}");

            try
            {
                if (itemDto == null)
                {
                    _logger.LogError("Invalid item.");
                    throw new ArgumentException("Invalid item. Enter proper details.");
                }

                FoodItem oldItem = await _foodItemRepository.GetByItemName(oldName);

                if (oldItem == null)
                {
                    _logger.LogError("Item not found.");
                    throw new Exception("Item not found.");
                }

                FoodItem existingItem = !string.IsNullOrEmpty(itemDto.ItemName) ? await _foodItemRepository.GetByItemName(itemDto.ItemName) : null;

                if (existingItem != null && existingItem.FoodItemId != oldItem.FoodItemId)
                {
                    _logger.LogError("Item already exists.");
                    throw new ArgumentException("This item name already exists.");
                }

                itemDto = UpdateItemFields(oldItem, itemDto, avilability);
                FoodItem item = _mapper.Map<FoodItemDTO, FoodItem>(itemDto);

                await _foodItemRepository.Update(item);

                response = ResponseHelper.CreateResponse("Update", "Updated succesfully");

                int notificationId = await _notificationService.AddNotification(new Notification
                {
                    UserId = UserData.UserId,
                    Message = $"The item {item.ItemName} has been updated. Check Details",
                    IsDelivered = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"message: {ex.Message}, DateTime: {DateTime.Now}");
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> Delete(string itemName)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Delete food item method called by userId: {UserData.UserId}, DateTime: {DateTime.Now}");

            try
            {
                FoodItem item = await _foodItemRepository.GetByItemName(itemName) ?? throw new Exception("Item not found.");

                await _foodItemRepository.Delete(item.FoodItemId);

                response  = ResponseHelper.CreateResponse("Delete", "Deleted successfully");
                _logger.LogInformation($"Item {item.ItemName} deleted successfully");
                
                int notificationId = await _notificationService.AddNotification(new Notification
                {
                    UserId = UserData.UserId,
                    Message = $"The item {item.ItemName} has been deleted from the main menu list.",
                    IsDelivered = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"message: {ex.Message}, DateTime: {DateTime.Now}");
                response= ResponseHelper.CreateResponse("Error",ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> GetFoodItemWithFeedbackReport(int month, int year)
        {
            ServerResponse serverResponse = new ServerResponse();
            _logger.LogInformation($"Get food item with feedback report method called by userId: {UserData.UserId}, DateTime: {DateTime.Now}");
            try
            {
                List<FoodItem> foodItems = (await _foodItemRepository.GetList()).ToList();
                List<int> foodItemIds = foodItems.Select(f => f.FoodItemId).ToList();

                List<Feedback> feedbacks = (await _feedbackRepository.GetList(
                                           predicate: f => foodItemIds.Contains(f.FoodItemId)
                                                        && f.FeedbackDate.Month == month
                                                        && f.FeedbackDate.Year == year
                                       )).ToList();

                if (feedbacks.IsNullOrEmpty())
                {
                    _logger.LogInformation("No feedbacks are given for the given month and year.");
                    return ResponseHelper.CreateResponse("Report", "No feedbacks are given for the given month and year.");
                }

                List<FoodReportDTO> foodItemWithFeedbacks = foodItems.Select(f =>
                {
                    List<Feedback> itemFeedbacks = feedbacks.Where(feedback => feedback.FoodItemId == f.FoodItemId).ToList();
                    double averageRating = itemFeedbacks.Any() ? Math.Round((double)itemFeedbacks.Average(feedback => feedback.Rating), 2) : 0;
                    List<string> comments = itemFeedbacks.Select(feedback => feedback.Comment).ToList();

                    return new FoodReportDTO
                    {
                        ItemName = f.ItemName,
                        Comments = comments,
                        AverageRating = averageRating
                    };
                }).ToList();

                serverResponse = ResponseHelper.CreateResponse("FoodItemWithFeedbackReport", JsonSerializer.Serialize(foodItemWithFeedbacks));
            }
            catch (Exception ex)
            {
                _logger.LogError($"message: {ex.Message}, DateTime: {DateTime.Now}");
                serverResponse = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return serverResponse;
        } 

        public async Task<ServerResponse> GetDiscardFoodItemList()
        {
            ServerResponse serverResponse = new ServerResponse();
            _logger.LogInformation($"Getdiscardfooditem list method called by userId: {UserData.UserId}, DateTime: {DateTime.Now}");
            
            try
            {
                List<FoodItem> foodItems = (await _foodItemRepository.GetList()).ToList();

                DateTime currentDate = DateTime.Now;

                List<Feedback> feedbacks = (await _feedbackRepository.GetList(predicate: f => f.FeedbackDate.Month == currentDate.Month && f.FeedbackDate.Year == currentDate.Year)).ToList();

                List<DiscardItemDTO> discardItems = foodItems.Select(f =>
                {
                    List<Feedback> itemFeedbacks = feedbacks.Where(feedback => feedback.FoodItemId == f.FoodItemId).ToList();
                    double averageRating = itemFeedbacks.Any() ? Math.Round((double)itemFeedbacks.Average(feedback => feedback.Rating), 2) : 0;

                    return new DiscardItemDTO
                    {
                        ItemName = f.ItemName,
                        AverageRating = averageRating
                    };
                }).ToList();

                discardItems = discardItems.Where(d => d.AverageRating <= 2).ToList();
                serverResponse = ResponseHelper.CreateResponse("DiscardItemList", JsonSerializer.Serialize(discardItems));
            }
            catch (Exception ex)
            {
                _logger.LogError($"message: {ex.Message}, DateTime: {DateTime.Now}");
                serverResponse = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return serverResponse;
        }

        public async Task<ServerResponse> DeleteDiscardMenu(string itemName)
        {
            var response = await this.GetDiscardFoodItemList();
            if (response.Name == "Error")
            {
                return response;
            }

            List<DiscardItemDTO> discardItems;
            try
            {
                string jsonString = response.Value as string;

                if (jsonString == null)
                {
                    _logger.LogError($"Expected response value to be a JSON string but got null, DateTime: {DateTime.Now}");
                    return ResponseHelper.CreateResponse("Error", "Invalid response value.");
                }

                discardItems = JsonSerializer.Deserialize<List<DiscardItemDTO>>(jsonString);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"Error deserializing discard items: {jsonEx.Message}, DateTime: {DateTime.Now}");
                return ResponseHelper.CreateResponse("Error", "Failed to parse discard items.");
            }

            DiscardItemDTO discardItem = discardItems.FirstOrDefault(d => d.ItemName == itemName);
            if (discardItem == null)
            {
                return ResponseHelper.CreateResponse("Error", "Item not found in the discard list.");
            }

            return await this.Delete(itemName);
        }

        private static FoodItemDTO UpdateItemFields(FoodItem oldItem, FoodItemDTO itemDto, string availability)
        {
            itemDto.FoodItemId = oldItem.FoodItemId;
            itemDto.ItemName = !string.IsNullOrEmpty(itemDto.ItemName) ? itemDto.ItemName : oldItem.ItemName;
            itemDto.Price = itemDto.Price != default ? itemDto.Price : oldItem.Price;
            itemDto.FoodCategory = itemDto.FoodCategory != default ? itemDto.FoodCategory : oldItem.FoodCategory;
            itemDto.IsAvailable = !string.IsNullOrEmpty(availability) ? availability.ToLower() == "y" : oldItem.IsAvailable;

            return itemDto;
        }
    }
}
