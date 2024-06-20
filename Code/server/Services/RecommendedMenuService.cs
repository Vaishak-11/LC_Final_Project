using AutoMapper;
using Microsoft.Extensions.Logging;
using RecommendationEngineServer.Helpers;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Models.Enums;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

namespace RecommendationEngineServer.Services
{
    public class RecommendedMenuService : IRecommendedMenuService
    {
        private readonly IRecommendedMenuRepository _recommendedMenuRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IFoodItemRepository _foodItemRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ILogger<RecommendedMenuService> _logger;

        public RecommendedMenuService(IRecommendedMenuRepository recommendedMenuRepository, IFoodItemRepository foodItemRepository, IMapper mapper, INotificationService notificationService, IFeedbackRepository feedbackRepository, ILogger<RecommendedMenuService> logger)
        {
            _recommendedMenuRepository = recommendedMenuRepository;
            _foodItemRepository = foodItemRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _feedbackRepository = feedbackRepository;
            _logger = logger;
        }

        public async Task<ServerResponse> AddRecommendedMenu(List<RecommendedMenuDTO> recommendations)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Adding recommended items {recommendations.Select(r=>r.ItemName)}by {recommendations.First().UserId}");

            try
            {
                if (recommendations == null || !recommendations.Any())
                {
                    _logger.LogError("Invalid recommendations.");
                    throw new ArgumentException("Invalid recommendations. Please provide valid data.");
                }

                List<string> itemNames = recommendations.Select(r => r.ItemName).ToList();
                List<FoodItem> existingItems = await _foodItemRepository.GetByItemNames(itemNames);
                List<string> nonexistingItems = itemNames.Except(existingItems.Select(m => m.ItemName)).ToList();


                if (nonexistingItems.Any())
                {
                    string nonexistingItemsList = string.Join(", ", nonexistingItems);
                    _logger.LogError($"These item names do not exist: {nonexistingItemsList}");
                    throw new Exception($"These item names do not exist: {nonexistingItemsList}");
                }

                List<RecommendedMenu> recommendationListToAdd = new List<RecommendedMenu>();

                foreach (var dto in recommendations)
                {
                    FoodItem item = existingItems.First(e => e.ItemName == dto.ItemName);
                    RecommendedMenu newRecommendation = _mapper.Map<RecommendedMenuDTO, RecommendedMenu>(dto);
                    newRecommendation.FoodItemId = item.FoodItemId;
                    recommendationListToAdd.Add(newRecommendation);
                }

                int success = await _recommendedMenuRepository.AddRange(recommendationListToAdd);

                if (success > 0)
                {
                    response = ResponseHelper.CreateResponse("AddRecommendedItems", "Recommended items added successfully.");
                    _logger.LogInformation($"Recommended items added successfully.");

                    int notificationId = await _notificationService.AddNotification(new Notification
                    {
                        UserId = recommendations.First().UserId,
                        Message = "The recommended menu for the upcoming day has been added.",
                        IsDelivered = false
                    });
                }
                else
                {
                    _logger.LogError("Adding recommended items failed.");
                    response = ResponseHelper.CreateResponse("Error", "Adding recommended items failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                response = ResponseHelper.CreateResponse("Error", $"An error occurred: {ex.Message}");
            }

            return response;
        }

        public async Task<ServerResponse> GetRecommendedMenu(DateTime? date = null)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Getting recommended items by userId: {UserData.UserId} for {date}");

            try
            {
                List<RecommendedMenu> recommendationList = (await _recommendedMenuRepository.GetListByDate(date)).ToList();

                if (!recommendationList.Any())
                {
                    _logger.LogError("No Menu is added to display.");
                    return ResponseHelper.CreateResponse("GetRecommendedMenu", "No Menu is added to display.");
                }

                List<int> itemIds = recommendationList.Select(r => r.FoodItemId).ToList();
                List<FoodItem> foodItems = (await _foodItemRepository.GetList(f => itemIds.Contains(f.FoodItemId))).ToList();

                List<DisplayRecommendedMenuDTO> displayMenuList = new List<DisplayRecommendedMenuDTO>();

                foreach (var recommendedMenu in recommendationList)
                {
                    FoodItem item = foodItems.FirstOrDefault(f => f.FoodItemId == recommendedMenu.FoodItemId);

                    if (item != null)
                    {
                        var feedbacks = await _feedbackRepository.GetList(f => f.FoodItemId == item.FoodItemId);
                        double averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
                        List<string> comments = feedbacks.Any() ? feedbacks.Select(f => f.Comment).ToList() : new List<string>();
                        string overallRating = await SentimentAnlysisHelper.AnalyzeSentiments(comments, averageRating);
                        List<string> selectedComments = await SelectComments(comments, overallRating);

                        displayMenuList.Add(new DisplayRecommendedMenuDTO
                        {
                            ItemName = item.ItemName,
                            Price = item.Price,
                            FoodCategory = recommendedMenu.Category,
                            Rating = averageRating,
                            Comments = comments,
                            OverallRating = overallRating
                        });
                    }
                }

                displayMenuList = displayMenuList
                            .OrderBy(d => d.FoodCategory)
                            .ThenByDescending(d => d.OverallRating)
                            .ToList();

                response.Name = "recommendedItemsList";
                response.Value = JsonSerializer.Serialize(displayMenuList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                response = ResponseHelper.CreateResponse("Error", $"An error occurred: {ex.Message}");
            }

            return response;
        }


        public async Task<ServerResponse> UpdateRecommendedMenu(RecommendedMenuDTO recommendedMenu)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Updating recommended item {recommendedMenu.ItemName} by userId: {UserData.UserId}");

            try
            {
                if (recommendedMenu == null)
                {
                    _logger.LogError("Invalid recommended menu details.");
                    throw new ArgumentException("Invalid recommended menu details. Please provide valid data.");
                }

                FoodItem existingItem = await _foodItemRepository.GetByItemName(recommendedMenu.OldItemName) ?? throw new Exception("Existing item name not found.");
                FoodItem newItem = await _foodItemRepository.GetByItemName(recommendedMenu.ItemName) ?? throw new InvalidOperationException("New item name not found");

                RecommendedMenu existingRecommendedMenu = await _recommendedMenuRepository.GetByItemId(existingItem.FoodItemId, recommendedMenu.OldCategory, recommendedMenu.RecommendationDate) ?? throw new Exception("The item you tried to modify is either not present in the category you mentioned or in the menu");

                existingRecommendedMenu.FoodItemId = newItem.FoodItemId;
                existingRecommendedMenu.UserId = recommendedMenu.UserId != default(int) ? recommendedMenu.UserId : existingRecommendedMenu.UserId;
                existingRecommendedMenu.Category = recommendedMenu.Category != default(FoodCategory) ? recommendedMenu.Category : existingRecommendedMenu.Category;
                existingRecommendedMenu.IsRecommended = recommendedMenu.IsRecommended;

                await _recommendedMenuRepository.Update(existingRecommendedMenu);

                response = ResponseHelper.CreateResponse("Update", "Updated successfully");
                _logger.LogInformation("Recommended item updated successfully.");

                int notificationId = await _notificationService.AddNotification(new Notification
                {
                    UserId = UserData.UserId,
                    Message = $"The recommended menu has been updated. Check out the details.",
                    IsDelivered = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                response = ResponseHelper.CreateResponse("Error", $"An error occurred: {ex.Message}");
            }

            return response;
        }

        private async Task<List<string>> SelectComments(List<string> comments, string overallRating)
        {
            List<string> selectedComments = new List<string>();
            Expression<Func<string, bool>> positiveCommentPredicate = (c => !string.IsNullOrWhiteSpace(c) && !c.ToLower().Contains("not") && SentimentAnlysisHelper.ContainsPositiveWord(c));
            Expression<Func<string, bool>> negativeCommentPredicate = (c => !string.IsNullOrWhiteSpace(c) && !c.ToLower().Contains("not") && SentimentAnlysisHelper.ContainsNegativeWord(c));

            overallRating = overallRating.ToLower() == "neutral" ? "0" : overallRating.ToLower() == "positive" ? "1" : "-1";

            if (overallRating.ToLower() == "positive" || Convert.ToDecimal(overallRating) > 0)
            {
                selectedComments = comments.AsQueryable().Where(positiveCommentPredicate).Take(3).ToList();
            }
            else if (overallRating.ToLower() == "Negative" || Convert.ToDecimal(overallRating) < 0)
            {
                selectedComments = comments.AsQueryable().Where(negativeCommentPredicate).Take(3).ToList();
            }
            else
            {
                List<string> positiveComments = comments.AsQueryable().Where(positiveCommentPredicate).Take(2).ToList();
                List<string> negativeComments = comments.AsQueryable().Where(negativeCommentPredicate).Take(2).ToList();

                selectedComments.AddRange(positiveComments);
                selectedComments.AddRange(negativeComments);
            }

            return selectedComments;
        }
    }
}
