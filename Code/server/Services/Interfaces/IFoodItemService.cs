using RecommendationEngineServer.Models.DTOs;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IFoodItemService
    {
        Task<ServerResponse> Add(FoodItemDTO menu);

        Task<ServerResponse> GetList();

        Task<ServerResponse> Update(string oldName, FoodItemDTO menuDto, string availability);

        Task<ServerResponse> Delete(string itemName);

        Task<ServerResponse> GetFoodItemWithFeedbackReport(int month, int year);

        Task<ServerResponse> GetDiscardFoodItemList();

        Task<ServerResponse> DeleteDiscardMenu(string itemName);

    }
}
