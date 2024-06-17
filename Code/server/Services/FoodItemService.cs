using AutoMapper;
using Castle.Core.Internal;
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

            if (item != null)
            {
                FoodItem existingItem = await _foodItemRepository.GetByItemName(item.ItemName);

                if (existingItem != null)
                {
                    response.Name = "Error";
                    response.Value = "This itemName already exists";

                    return response;
                }

                FoodItem newItem = _mapper.Map<FoodItemDTO, FoodItem>(item);
                int itemId = await _foodItemRepository.Add(newItem);

                response.Name = "AddItem";
                response.Value = (itemId > 0) ? "Item added successfully." : "Adding Item failed.";

                if (itemId > 0)
                {
                    int notificationId = await _notificationService.AddNotification(new Notification
                    {
                        UserId = UserData.UserId,
                        Message = $"New item {item.ItemName} added to the menu list.",
                        IsDelivered = false
                    });
                }
            }
            else
            {
                response.Name = "Error";
                response.Value = "Invalid item. Enter proper details";
            }

            return response;
        }

        public async Task<ServerResponse> GetList()
        {
            ServerResponse response = new ServerResponse();

            List<FoodItem> itemList = (await _foodItemRepository.GetList()).ToList();
            List<int> itemIds = itemList.Select(i => i.FoodItemId).ToList();

            List<Feedback> feedbackList = (await _feedbackRepository.GetList(predicate: f => itemIds.Contains(f.FoodItemId))).ToList();

            List<DisplayMenuDTO> items = itemList.Select(foodItem =>
            {
                var feedbacks = feedbackList.Where(f => f.FoodItemId == foodItem.FoodItemId).ToList();
                double averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;

                return new DisplayMenuDTO
                {
                    ItemName = foodItem.ItemName,
                    Price = foodItem.Price,
                    Rating = averageRating
                };
            }).ToList();

            response.Name = "itemList";
            response.Value = !itemList.IsNullOrEmpty() ? JsonSerializer.Serialize(items) : new List<DisplayMenuDTO>();

            return response;
        }

        public async Task<ServerResponse> Update(string oldName, FoodItemDTO itemDto, string avilability)
        {
            ServerResponse response = new ServerResponse();

            if (itemDto != null)
            {
                FoodItem oldItem = await _foodItemRepository.GetByItemName(oldName);
                if (oldItem != null)
                {
                    FoodItem existingItem = itemDto.ItemName != null ? await _foodItemRepository.GetByItemName(itemDto.ItemName) : null;

                    if (existingItem != null && existingItem.FoodItemId != oldItem.FoodItemId)
                    {
                        response.Name = "Error";
                        response.Value = "This itemName already exists";

                        return response;
                    }

                    itemDto = UpdateItemFields(oldItem, itemDto, avilability);

                    FoodItem item = _mapper.Map<FoodItemDTO, FoodItem>(itemDto);
                    try
                    {
                        await _foodItemRepository.Update(item);
                        response.Name = "Update";
                        response.Value = "Updated succesfully";

                        int notificationId = await _notificationService.AddNotification(new Notification
                        {
                            UserId = UserData.UserId,
                            Message = $"The item {item.ItemName} has been updated. Check Details",
                            IsDelivered = false
                        });
                    }
                    catch (Exception ex)
                    {
                        response.Name = "Error";
                        response.Value = ex.Message;
                    }
                }
                else
                {
                    response.Name = "Error";
                    response.Value = "Item not found";
                }
            }
            else
            {
                response.Name = "Error";
                response.Value = "Invalid item. Enter proper details";
            }

            return response;
        }

        public async Task<ServerResponse> Delete(string itemName)
        {
            ServerResponse response = new ServerResponse();

            FoodItem item = await _foodItemRepository.GetByItemName(itemName);

            if (item != null)
            {
                try
                {
                    await _foodItemRepository.Delete(item.FoodItemId);
                    response.Name = "Delete";
                    response.Value = "Deleted successfully";

                    int notificationId = await _notificationService.AddNotification(new Notification
                    {
                        UserId = UserData.UserId,
                        Message = $"The item {item.ItemName} has been deleted from the main menu list.",
                        IsDelivered = false
                    });
                }
                catch (Exception ex)
                {
                    response.Name = "Error";
                    response.Value = ex.Message;
                }
            }
            else
            {
                response.Name = "Error";
                response.Value = "Item not found";
            }

            return response;
        }

        public async Task<ServerResponse> GetFoodItemWithFeedbackReport(int month, int year)
        {
            List<FoodItem> foodItems = (await _foodItemRepository.GetList()).ToList();
            List<int> foodItemIds = foodItems.Select(f => f.FoodItemId).ToList();

            List<Feedback> feedbacks = (await _feedbackRepository.GetList(
                                           predicate: f => foodItemIds.Contains(f.FoodItemId)
                                                        && f.FeedbackDate.Month == month
                                                        && f.FeedbackDate.Year == year
                                       )).ToList();

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

            ServerResponse response = new ServerResponse
            {
                Name = "FoodItemWithFeedbackReport",
                Value = JsonSerializer.Serialize(foodItemWithFeedbacks)
            };

            return response;
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
