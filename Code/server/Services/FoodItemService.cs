using AutoMapper;
using Castle.Core.Internal;
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

        public FoodItemService(IFoodItemRepository foodItemRepository, IMapper mapper, INotificationService notificationService, IFeedbackRepository feedbackRepository)
        {
            _foodItemRepository = foodItemRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<ServerResponse> Add(FoodItemDTO item)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                if (item == null)
                {
                    throw new ArgumentException("Invalid item. Enter proper details.");
                }

                var existingItem = await _foodItemRepository.GetByItemName(item.ItemName);

                if (existingItem != null)
                {
                    throw new ArgumentException("This item name already exists.");
                }

                var newItem = _mapper.Map<FoodItem>(item);
                var itemId = await _foodItemRepository.Add(newItem);

                string message = itemId > 0 ? "Item added successfully." : "Adding item failed.";
                response = ResponseHelper.CreateResponse("AddItem", message);

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
            }

            return response;
        }

        public async Task<ServerResponse> GetList()
        {
            var response = new ServerResponse();

            try
            {
                List<FoodItem> itemList = (await _foodItemRepository.GetList()).ToList();

                if (!itemList.Any())
                {
                    throw new Exception("No items found.");
                }

                var itemIds = itemList.Select(i => i.FoodItemId).ToList();
                var feedbackList = (await _feedbackRepository.GetList(f => itemIds.Contains(f.FoodItemId))).ToList();

                var itemTasks = itemList.Select(async foodItem =>
                {
                    var feedbacks = feedbackList.Where(f => f.FoodItemId == foodItem.FoodItemId).ToList();
                    var averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
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
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> Update(string oldName, FoodItemDTO itemDto, string avilability)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                if (itemDto == null)
                {
                    throw new ArgumentException("Invalid item. Enter proper details.");
                }

                FoodItem oldItem = await _foodItemRepository.GetByItemName(oldName);

                if (oldItem == null)
                {
                    throw new Exception("Item not found.");
                }

                FoodItem existingItem = !string.IsNullOrEmpty(itemDto.ItemName) ? await _foodItemRepository.GetByItemName(itemDto.ItemName) : null;

                if (existingItem != null && existingItem.FoodItemId != oldItem.FoodItemId)
                {
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
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> Delete(string itemName)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                FoodItem item = await _foodItemRepository.GetByItemName(itemName) ?? throw new Exception("Item not found.");
;
                await _foodItemRepository.Delete(item.FoodItemId);

                response  = ResponseHelper.CreateResponse("Delete", "Deleted successfully");
                
                int notificationId = await _notificationService.AddNotification(new Notification
                {
                    UserId = UserData.UserId,
                    Message = $"The item {item.ItemName} has been deleted from the main menu list.",
                    IsDelivered = false
                });
            }
            catch (Exception ex)
            {
                response= ResponseHelper.CreateResponse("Error",ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> GetFoodItemWithFeedbackReport(int month, int year)
        {
            ServerResponse serverResponse = new ServerResponse();
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
                    return ResponseHelper.CreateResponse("Report", "No feedbacks are given for the given month and year.");
                }

                List<FoodReportDTO> foodItemWithFeedbacks = foodItems.Select(f =>
                {
                    List<Feedback> itemFeedbacks = feedbacks.Where(feedback => feedback.FoodItemId == f.FoodItemId).ToList();
                    double averageRating = itemFeedbacks.Any() ? itemFeedbacks.Average(feedback => feedback.Rating) : 0;
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
                serverResponse = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return serverResponse;
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
