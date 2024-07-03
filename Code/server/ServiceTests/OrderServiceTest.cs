using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using ServerUnitTests.testData;
using System.Linq.Expressions;
using System.Text.Json;

namespace ServerUnitTests.ServiceTests
{
    [TestClass]
    public class OrderServiceTest
    {
        #region private properties

        private OrderService _orderService;
        private Mock<IOrderRepository> _mockOrderRepository;
        private Mock<IFoodItemRepository> _mockFoodItemRepository;
        private Mock<IOrderItemRepository> _mockOrderItemRepository;
        private Mock<IRecommendedMenuRepository> _mockRecommendedMenuRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<OrderService>> _mockLogger;

        #endregion private properties

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockFoodItemRepository = new Mock<IFoodItemRepository>();
            _mockOrderItemRepository = new Mock<IOrderItemRepository>();
            _mockRecommendedMenuRepository = new Mock<IRecommendedMenuRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                               _mockOrderRepository.Object,
                               _mockOrderItemRepository.Object,
                               _mockFoodItemRepository.Object,
                               _mockRecommendedMenuRepository.Object,
                               _mockLogger.Object);
        }

        #endregion Constructor

        #region Test Methods

        #region AddOrder Method

        [TestMethod]
        public async Task AddOrder_InvalidOrder_ShouldReturnErrorResponse()
        {
            var orderDto = (OrderDTO)null;
            var response = await _orderService.AddOrder(orderDto);

            Assert.AreEqual("Error", response.Name);
            Assert.AreEqual("Invalid order details. Enter proper details.", response.Value);
        }

        [TestMethod]
        public async Task AddOrder_NonExistingItems_ShouldReturnErrorResponse()
        {
            var orderDto = OrderServiceTestData.OrderDTOs().FirstOrDefault();

            _mockFoodItemRepository.Setup(m => m.GetByItemNames(It.IsAny<List<string>>())).ReturnsAsync(new List<FoodItem>());
            var response = await _orderService.AddOrder(orderDto);

            Assert.AreEqual("Error", response.Name);
            Assert.AreEqual($"These item names do not exist: Dosa, Rice", response.Value);
        }

        [TestMethod]
        public async Task AddOrder_NoRecommendedMenus_ShouldReturnErrorResponse()
        {
            var orderDto = OrderServiceTestData.OrderDTOs().FirstOrDefault();
            var foodItems = OrderServiceTestData.FoodItems();

            _mockFoodItemRepository.Setup(m => m.GetByItemNames(It.IsAny<List<string>>())).ReturnsAsync(foodItems);
            _mockRecommendedMenuRepository.Setup(m => m.GetByItemIds(It.IsAny<List<int>>())).ReturnsAsync(new List<RecommendedMenu>());
            var response = await _orderService.AddOrder(orderDto);

            Assert.AreEqual("Error", response.Name);
            Assert.AreEqual("No recommended menus found for the given items.", response.Value);
        }

        [TestMethod]
        public async Task AddOrder_ValidOrder_ShouldReturnSuccessResponse()
        {
            var orderDto = OrderServiceTestData.OrderDTOs().FirstOrDefault();
            var foodItems = OrderServiceTestData.FoodItems();
            var recommendedMenus = OrderServiceTestData.RecommendedMenus();

            _mockFoodItemRepository.Setup(m => m.GetByItemNames(It.IsAny<List<string>>())).ReturnsAsync(foodItems);
            _mockRecommendedMenuRepository.Setup(m => m.GetByItemIds(It.IsAny<List<int>>())).ReturnsAsync(recommendedMenus);
            _mockOrderRepository.Setup(m => m.Add(It.IsAny<Order>())).ReturnsAsync(1);

            var response = await _orderService.AddOrder(orderDto);

            Assert.AreEqual("AddOrder", response.Name);
            Assert.AreEqual("Order added successfully.", response.Value);
        }

        #endregion AddOrder Method

        #region GetOrders Method

        [TestMethod]
        public async Task GetOrders_ShouldReturnOrders()
        {
            var orders = OrderServiceTestData.Orders();
            var orderItems = OrderServiceTestData.OrderItems();
            var recommendedMenus = OrderServiceTestData.RecommendedMenus();
            var foodItems = OrderServiceTestData.FoodItems();

            _mockOrderRepository.Setup(m => m.GetListByDate(It.IsAny<DateTime?>())).ReturnsAsync(orders);
            _mockOrderItemRepository.Setup(m => m.GetList(It.IsAny<Expression<Func<OrderItem,bool>>>())).ReturnsAsync(orderItems);
            _mockRecommendedMenuRepository.Setup(m => m.GetList(It.IsAny<Expression<Func<RecommendedMenu, bool>>>())).ReturnsAsync(recommendedMenus);
            _mockFoodItemRepository.Setup(m => m.GetList(It.IsAny<Expression<Func<FoodItem, bool>>>())).ReturnsAsync(foodItems);

            var response = await _orderService.GetOrders();

            Assert.AreEqual("Order List", response.Name);

            Assert.IsNotNull(response.Value);

            var orderList = JsonSerializer.Deserialize<List<DisplayFeedbackDTO>>(response.Value.ToString());
            Assert.AreEqual(2, orderList.Count);
        }

        [TestMethod]
        public async Task GetOrders_WhenThereAreNoOrders_ShouldReturnEmptyOrderList()
        {
            var orders = new List<Order>();

            _mockOrderRepository.Setup(m => m.GetListByDate(It.IsAny<DateTime?>())).ReturnsAsync(orders);

            var response = await _orderService.GetOrders();

            Assert.AreEqual("GetOrders", response.Name);
            Assert.IsNotNull(response.Value);
            Assert.AreEqual("No orders found.", response.Value.ToString());
        }

        #endregion GetOrders Method

        #endregion Test Methods

    }
}
