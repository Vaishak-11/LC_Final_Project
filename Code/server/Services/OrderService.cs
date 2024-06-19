﻿using RecommendationEngineServer.Helpers;
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

        public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IFoodItemRepository foodItemRepository, IRecommendedMenuRepository recommendedMenuRepository)
        {
            _orderRepository = orderRepository;
            _foodItemRepository = foodItemRepository;
            _orderItemRepository = orderItemRepository;
            _recommendedMenuRepository = recommendedMenuRepository;
        }


        public async Task<ServerResponse> AddOrder(OrderDTO order)
        {
            ServerResponse response = new ServerResponse();

            try 
            {
                if (order == null || !order.ItemNames.Any())
                {
                    response =ResponseHelper.CreateResponse("Error", "Invalid order details. Enter proper details.");
                    return response;
                }

                List<FoodItem> existingItems = await _foodItemRepository.GetByItemNames(order.ItemNames);
                List<string> nonexistingItems = order.ItemNames.Except(existingItems.Select(m => m.ItemName)).ToList();

                if (nonexistingItems.Any())
                {
                    response.Name = "Error";
                    string nonexistingItemsList = string.Join(", ", nonexistingItems);
                    response.Value = $"These item names do not exist: {nonexistingItemsList}";

                    return response;
                }

                List<int> itemIds = existingItems.Select(item => item.FoodItemId).ToList();
                List<RecommendedMenu> recommendedMenus = await _recommendedMenuRepository.GetByItemIds(itemIds);

                if (!recommendedMenus.Any())
                {
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
                            return ResponseHelper.CreateResponse("Error", $"Recommended menu for food item '{itemName}' not found.");
                        }
                    }

                    response = ResponseHelper.CreateResponse("AddOrder", "Order added successfully.");
                }
                else
                {
                    response = ResponseHelper.CreateResponse("Error", "Failed to add order.");
                }
            }
            catch (Exception ex)
            {
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }

        public async Task<ServerResponse> GetOrders(DateTime? date = null)
        {
            ServerResponse response = new ServerResponse();

            try
            {
                List<Order> orders = ((await _orderRepository.GetListByDate(date)).ToList()) ?? throw new Exception("Failed to fetch orders.");

                if (!orders.Any())
                {
                    return ResponseHelper.CreateResponse("GetOrders", "No orders found.");
                }

                List<int> orderIds = orders.Select(o => o.OrderId).ToList();
                List<OrderItem> orderItems = (await _orderItemRepository.GetList(predicate: oi => orderIds.Contains(oi.OrderId))).ToList();

                if (!orderItems.Any())
                {
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
                response = ResponseHelper.CreateResponse("Error", ex.Message.ToString());
            }

            return response;
        }
    }
}
