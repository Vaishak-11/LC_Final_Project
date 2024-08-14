using RecommendationEngineServer.Models.DTOs;

namespace RecommendationEngineServer.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServerResponse> AddOrder(OrderDTO order);

        Task<ServerResponse> GetOrders(DateTime? date = null);
    }
}
