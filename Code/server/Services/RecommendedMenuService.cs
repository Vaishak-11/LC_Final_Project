using AutoMapper;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Models.Enums;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;
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

        public RecommendedMenuService(IRecommendedMenuRepository recommendedMenuRepository, IFoodItemRepository foodItemRepository, IMapper mapper, INotificationService notificationService, IFeedbackRepository feedbackRepository)
        {
            _recommendedMenuRepository = recommendedMenuRepository;
            _foodItemRepository = foodItemRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<ServerResponse> AddRecommendedMenu(List<RecommendedMenuDTO> recommendations)
        {
            ServerResponse response = new ServerResponse();

            if (recommendations != null && recommendations.Any())
            {
                List<string> itemNames = recommendations.Select(r => r.ItemName).ToList();
                List<FoodItem> existingItems = await _foodItemRepository.GetByItemNames(itemNames);
                List<string> nonexistingItems = itemNames.Except(existingItems.Select(m => m.ItemName)).ToList();

                if (nonexistingItems.Any())
                {
                    response.Name = "Error";
                    string nonexistingItemsList = string.Join(", ", nonexistingItems);
                    response.Value = $"These item names do not exist: {nonexistingItemsList}";

                    return response;
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
                response.Name = "AddRecommendedItems";
                response.Value = (success > 0) ? "Recommended Items added successfully." : "Adding Recommended items failed.";

                int notificationId = await _notificationService.AddNotification(new Notification
                {
                    UserId = recommendations.First().UserId,
                    Message = "The recommended menu for the upcoming day has been added.",
                    IsDelivered = false
                });
            }
            else
            {
                response.Name = "Error";
                response.Value = "Invalid item.Enter proper details";
            }

            return response;
        }

        public async Task<ServerResponse> GetRecommendedMenu(DateTime? date = null)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                List<RecommendedMenu> recommendationList = (await _recommendedMenuRepository.GetListByDate(date)).ToList();

                if (!recommendationList.Any())
                {
                    response.Name = "Success";
                    response.Value = "No Menu is added to display.";
                    return response;
                }

                List<int> itemIds = recommendationList.Select(r => r.FoodItemId).ToList();
                List<FoodItem> foodItems = (await _foodItemRepository.GetList(f => itemIds.Contains(f.FoodItemId))).ToList();

                List<DisplayMenuDTO> displayMenuList = new List<DisplayMenuDTO>();

                foreach (var recommendedMenu in recommendationList)
                {
                    FoodItem item = foodItems.FirstOrDefault(f => f.FoodItemId == recommendedMenu.FoodItemId);

                    if (item != null)
                    {
                        var feedbacks = await _feedbackRepository.GetList(f => f.FoodItemId == item.FoodItemId);
                        double averageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
                        List<string> comments = feedbacks.Select(f => f.Comment).ToList();

                        displayMenuList.Add(new DisplayMenuDTO
                        {
                            ItemName = item.ItemName,
                            Price = item.Price,
                            Rating = averageRating,
                            Comments = comments,
                            Sentiment = (await AnalyzeSentimentsAsync(comments)).ToString()
                        }); ;
                    }
                }

                response.Name = "recommendedItemsList";
                response.Value = JsonSerializer.Serialize(displayMenuList);
            }
            catch (Exception ex)
            {
                response.Name = "Error";
                response.Value = $"An error occurred: {ex.Message}";
            }

            return response;
        }


        public async Task<ServerResponse> UpdateRecommendedMenu(RecommendedMenuDTO recommendedMenu)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                FoodItem existingItem = await _foodItemRepository.GetByItemName(recommendedMenu.OldItemName);
                FoodItem newItem = await _foodItemRepository.GetByItemName(recommendedMenu.ItemName);

                if (existingItem == null)
                {
                    response.Name = "Error";
                    response.Value = "Existing item not found";
                    return response;
                }

                if (newItem == null)
                {
                    response.Name = "Error";
                    response.Value = "New item not found";
                    return response;
                }

                RecommendedMenu existingRecommendedMenu = await _recommendedMenuRepository.GetByItemId(existingItem.FoodItemId, recommendedMenu.OldCategory, recommendedMenu.RecommendationDate);

                if(recommendedMenu == null)
                {
                    response.Name = "Error";
                    response.Value = "The item you tried to modify is either not present in the categor you mentioned or in the menu";
                    return response;
                }
                existingRecommendedMenu.FoodItemId = newItem.FoodItemId;
                existingRecommendedMenu.UserId = recommendedMenu.UserId != default(int) ? recommendedMenu.UserId : existingRecommendedMenu.UserId;
                existingRecommendedMenu.Category = recommendedMenu.Category != default(FoodCategory) ? recommendedMenu.Category : existingRecommendedMenu.Category;
                existingRecommendedMenu.IsRecommended = recommendedMenu.IsRecommended;

                await _recommendedMenuRepository.Update(existingRecommendedMenu);

                response.Name = "Update";
                response.Value = "Updated successfully";

                int notificationId = await _notificationService.AddNotification(new Notification
                {
                    UserId = UserData.UserId,
                    Message = $"The recommended menu has been updated. Check out the details.",
                    IsDelivered = false
                });
            }
            catch (Exception ex)
            {
                response.Name = "Error";
                response.Value = ex.Message;
            }

            return response;
        }

        private async Task<Sentiment> AnalyzeSentimentsAsync(List<string> comments)
        {
            int sentimentScore = 0;

            foreach (var comment in comments)
            {
                string normalizedComment = comment.ToLower();

                if (normalizedComment.Contains("not"))
                {
                    await HandleNotComment(comment, sentimentScore);
                }
                else
                {
                    if (ContainsPositiveWord(normalizedComment) && !ContainsNegativeWord(normalizedComment))
                    {
                        ++sentimentScore;
                    }
                    else if (ContainsNegativeWord(normalizedComment) && !ContainsPositiveWord(normalizedComment))
                    {
                        --sentimentScore;
                    }
                }
            }

            Sentiment sentiment = sentimentScore == 0 ? Sentiment.Neutral : sentimentScore > 0 ? Sentiment.Positive : Sentiment.Negative;

            return sentiment;
        }

        private static async Task<int> HandleNotComment(string comment, int sentimentScore)
        {
            string[] words = comment.ToLower().Split(' ');

            bool isNegativeContext = false;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "not")
                {
                    isNegativeContext = true;
                    continue;
                }

                if (isNegativeContext)
                {
                    if (Enum.TryParse(words[i], true, out PositiveCommentWords positiveWord))
                    {
                        sentimentScore--;
                    }
                    else if (Enum.TryParse(words[i], true, out NegativeCommentWords negativeWord))
                    {
                        sentimentScore++;
                    }

                    isNegativeContext = false; 
                }
                else
                {
                    if (Enum.TryParse(words[i], true, out PositiveCommentWords positiveWord))
                    {
                        sentimentScore++;
                    }
                    else if (Enum.TryParse(words[i], true, out NegativeCommentWords negativeWord))
                    {
                        sentimentScore--;
                    }
                }
            }

            return sentimentScore;
        }

        private static bool ContainsPositiveWord(string comment)
        {
            foreach (PositiveCommentWords word in Enum.GetValues(typeof(PositiveCommentWords)))
            {
                if (comment.Contains(word.ToString().ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsNegativeWord(string comment)
        {
            foreach (NegativeCommentWords word in Enum.GetValues(typeof(NegativeCommentWords)))
            {
                if (comment.Contains(word.ToString().ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        
    }
}
