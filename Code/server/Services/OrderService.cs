using Microsoft.Extensions.Logging;
using RecommendationEngineServer.Helpers;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services.Interfaces;
using System.Text.Json;

namespace RecommendationEngineServer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IFoodItemRepository _foodItemRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IRecommendedMenuRepository _recommendedMenuRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IFoodItemRepository foodItemRepository, IRecommendedMenuRepository recommendedMenuRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _foodItemRepository = foodItemRepository;
            _orderItemRepository = orderItemRepository;
            _recommendedMenuRepository = recommendedMenuRepository;
            _logger = logger;
        }


        public async Task<ServerResponse> AddOrder(OrderDTO order)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Add order method called; UserId: {UserData.UserId} DateTime:{DateTime.Now}");

            try 
            {
                if (order == null || !order.ItemNames.Any())
                {
                    _logger.LogError("Invalid order details.");
                    response =ResponseHelper.CreateResponse("Error", "Invalid order details. Enter proper details.");
                    return response;
                }

                _logger.LogInformation($"Add order method called for items {order.ItemNames}; UserId: {UserData.UserId} DateTime:{DateTime.Now}");

                List<FoodItem> existingItems = await _foodItemRepository.GetByItemNames(order.ItemNames);
                List<string> nonexistingItems = order.ItemNames.Except(existingItems.Select(m => m.ItemName)).ToList();

                if (nonexistingItems.Any())
                {
                    response.Name = "Error";
                    string nonexistingItemsList = string.Join(", ", nonexistingItems);
                    response.Value = $"These item names do not exist: {nonexistingItemsList}";
                    _logger.LogError($"These item names do not exist: {nonexistingItemsList}");

                    return response;
                }

                List<int> itemIds = existingItems.Select(item => item.FoodItemId).ToList();
                List<RecommendedMenu> recommendedMenus = await _recommendedMenuRepository.GetByItemIds(itemIds);

                if (!recommendedMenus.Any())
                {
                    _logger.LogError("No recommended menus found for the given items.");
                    return ResponseHelper.CreateResponse("Error", "No recommended menus found for the given items.");
                }

                Order newOrder = new Order
                {
                    UserId = order.UserId,
                    OrderDate = DateTime.Now
                };

                int orderId = await _orderRepository.Add(newOrder);
                if (orderId > 0)
                {
                    foreach (var itemName in order.ItemNames)
                    {
                        FoodItem foodItem = existingItems.FirstOrDefault(item => item.ItemName == itemName);
                        RecommendedMenu recommendedMenu = recommendedMenus.FirstOrDefault(menu => menu.FoodItemId == foodItem.FoodItemId);

                        if (foodItem != null && recommendedMenu != null)
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                OrderId = orderId,
                                MenuId = recommendedMenu.MenuId
                            };

                            await _orderItemRepository.Add(orderItem);
                        }
                        else
                        {
                            _logger.LogError($"Recommended menu for food item '{itemName}' not found.");
                            return ResponseHelper.CreateResponse("Error", $"Recommended menu for food item '{itemName}' not found.");
                        }
                    }

                    response = ResponseHelper.CreateResponse("AddOrder", "Order added successfully.");
                }
                else
                {
                    _logger.LogError("Failed to add order.");
                    response = ResponseHelper.CreateResponse("Error", "Failed to add order.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding order: {ex.Message}");
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> GetOrders(DateTime? date = null)
        {
            ServerResponse response = new ServerResponse();
            _logger.LogInformation($"Get orders method called by userId: {UserData.UserId} for {date}; DateTime:{DateTime.Now}");

            try
            {
                List<Order> orders = ((await _orderRepository.GetListByDate(date)).ToList());

                if (orders == null || !orders.Any())
                {
                    _logger.LogError("No orders found.");
                    return ResponseHelper.CreateResponse("GetOrders", "No orders found.");
                }

                List<int> orderIds = orders.Select(o => o.OrderId).ToList();
                List<OrderItem> orderItems = (await _orderItemRepository.GetList(predicate: oi => orderIds.Contains(oi.OrderId))).ToList();

                if (!orderItems.Any())
                {
                    _logger.LogError("No order items found.");
                    return ResponseHelper.CreateResponse("GetOrders", "No order items found.");
                }

                List<int> menuIds = orderItems.Select(oi => oi.MenuId).ToList();
                List<RecommendedMenu> recommendedMenus = (await _recommendedMenuRepository.GetList(rm => menuIds.Contains(rm.MenuId))).ToList();

                List<int> foodItemIds = orderItems.Select(oi => oi.RecommendedMenu.FoodItemId).ToList();
                List<FoodItem> foodItems = (await _foodItemRepository.GetList(fi => foodItemIds.Contains(fi.FoodItemId))).ToList();

                List<DisplayOrderDTO> orderDtoList = new List<DisplayOrderDTO>();

                foreach (var order in orders)
                {
                    var orderItemsForOrder = orderItems.Where(oi => oi.OrderId == order.OrderId).ToList();

                    foreach (var orderItem in orderItemsForOrder)
                    {
                        var recommendedMenu = recommendedMenus.FirstOrDefault(rm => rm.MenuId == orderItem.MenuId);

                        if (recommendedMenu != null)
                        {
                            var foodItem = foodItems.FirstOrDefault(fi => fi.FoodItemId == recommendedMenu.FoodItemId);

                            if (foodItem != null)
                            {
                                orderDtoList.Add(new DisplayOrderDTO
                                {
                                    UserName = order.User.UserName,
                                    ItemName = foodItem.ItemName
                                });
                            }
                        }
                    }
                }

                var groupedOrderDtoList = orderDtoList
                                            .GroupBy(o => o.ItemName)
                                            .Select(group => new DisplayOrderDTO
                                            {
                                                ItemName = group.Key,
                                                UserName = string.Join(", ", group.Select(o => o.UserName))
                                            })
                                            .ToList();

                response.Name = "Order List";
                response.Value = JsonSerializer.Serialize(groupedOrderDtoList);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error fetching orders: {ex.Message}");
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }
    }
}
