using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Models.Enums;
using RecommendationEngineServer.Profiles;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Services.Interfaces;
using ServerUnitTests.testData;
using System.Linq.Expressions;

namespace ServerUnitTests.ServiceTests
{
    [TestClass]
    public class RecommendedMenuServiceTest
    {
        private RecommendedMenuService _recommendedMenuService;
        private Mock<IRecommendedMenuRepository> _mockRecommendedMenuRepository;
        private Mock<INotificationService> _mockNotificationService;
        private IMapper _mapper;
        private Mock<IFoodItemRepository> _mockFoodItemRepository;
        private Mock<IFeedbackRepository> _mockFeedbackRepository;
        private Mock<ILogger<RecommendedMenuService>> _mockLogger;
        private Mock<IEmployeeRepository> _mockEmployeeRepository;

        [TestInitialize]
        public void Initialize()
        {
            _mockRecommendedMenuRepository = new Mock<IRecommendedMenuRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockFoodItemRepository = new Mock<IFoodItemRepository>();
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();
            _mockLogger = new Mock<ILogger<RecommendedMenuService>>();
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfile>();
            });

            _mapper = config.CreateMapper();

            _recommendedMenuService = new RecommendedMenuService(
                _mockRecommendedMenuRepository.Object,
                _mockFoodItemRepository.Object,
                _mapper,
                _mockNotificationService.Object,
                _mockFeedbackRepository.Object,
                _mockEmployeeRepository.Object,
                _mockLogger.Object
            );
        }

        #region AddMethod

        [TestMethod]
        public async Task AddRecommendedMenu_ValidRecommendations_ReturnsSuccessResponse()
        {
            var recommendations = (RecommendedMenuTestData.RecommendedMenuDTOs()).ToList();

            var existingItems = RecommendedMenuTestData.FoodItems();

            _mockFoodItemRepository.Setup(repo => repo.GetByItemNames(It.IsAny<List<string>>()))
                .ReturnsAsync(existingItems);

            _mockRecommendedMenuRepository.Setup(repo => repo.AddRange(It.IsAny<List<RecommendedMenu>>()))
                .ReturnsAsync(2);

            _mockNotificationService.Setup(service => service.AddNotification(It.IsAny<Notification>()))
                .ReturnsAsync(1);

            var result = await _recommendedMenuService.AddRecommendedMenu(recommendations);

            Assert.AreEqual("AddRecommendedItems", result.Name);
            Assert.AreEqual("Recommended items added successfully.", result.Value);
        }

        [TestMethod]
        public async Task AddRecommendedMenu_InvalidRecommendations_ReturnsErrorResponse()
        {
            var recommendations = new List<RecommendedMenuDTO>();

            var result = await _recommendedMenuService.AddRecommendedMenu(recommendations);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("An error occurred: Invalid recommendations. Please provide valid data.", result.Value);
        }

        [TestMethod]
        public async Task AddRecommendedMenu_NonExistingItems_ReturnsErrorResponse()
        {
            var recommendations = new List<RecommendedMenuDTO>
            {
                new RecommendedMenuDTO { ItemName = "Item1", Category = FoodCategory.Breakfast, UserId = 1 }
            };

            var existingItems = new List<FoodItem>();

            _mockFoodItemRepository.Setup(repo => repo.GetByItemNames(It.IsAny<List<string>>()))
                .ReturnsAsync(existingItems);

            var result = await _recommendedMenuService.AddRecommendedMenu(recommendations);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("An error occurred: These item names do not exist: Item1", result.Value);
        }


        #endregion AddMethod

        #region GetRecommendedMenuMethod

        [TestMethod]
        public async Task GetRecommendedMenu_WithValidInputForEmployee_ReturnsSuccessResponse()
        {
            var date = DateTime.Now;
            var recommendedMenus = RecommendedMenuTestData.RecommendedMenuData().ToList();
            var employee = RecommendedMenuTestData.Employees().FirstOrDefault();

            _mockRecommendedMenuRepository.Setup(repo => repo.GetListByDate(date, It.IsAny<string>()))
                .ReturnsAsync(recommendedMenus);

            _mockEmployeeRepository.Setup(repo => repo.GetList(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(new List<Employee> { employee });

            var result = await _recommendedMenuService.GetRecommendedMenu(date);

            Assert.AreEqual("recommendedItemsList", result.Name);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value.ToString()));
        }

        [TestMethod]
        public async Task GetRecommendedMenu_WithValidInputForChef_ReturnsSuccessResponse()
        {
            var date = DateTime.Now;
            var recommendedMenus = RecommendedMenuTestData.RecommendedMenuData().ToList();
            var employee = RecommendedMenuTestData.Employees().FirstOrDefault();

            _mockRecommendedMenuRepository.Setup(repo => repo.GetListByDate(date, It.IsAny<string>()))
                .ReturnsAsync(recommendedMenus);

            _mockEmployeeRepository.Setup(repo => repo.GetList(It.IsAny<Expression<Func<Employee, bool>>>()))
                .ReturnsAsync(new List<Employee>());

            var result = await _recommendedMenuService.GetRecommendedMenu(date);

            Assert.AreEqual("recommendedItemsList", result.Name);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value.ToString()));
        }

        [TestMethod]
        public async Task GetRecommendedMenu_WithNoRecommendations_ReturnsEmptyList()
        {
            var date = DateTime.Now;

            _mockRecommendedMenuRepository.Setup(repo => repo.GetListByDate(date, It.IsAny<string>()))
                .ReturnsAsync(new List<RecommendedMenu>());

            var result = await _recommendedMenuService.GetRecommendedMenu(date);

            Assert.AreEqual("GetRecommendedMenu", result.Name);
            Assert.AreEqual("No Menu is added to display.", result.Value);
        }

        #endregion GetRecommendedMenuMethod

        #region UpdateRecommendedMenuMethod

        [TestMethod]
        public async Task UpdateRecommendedMenu_ValidMenu_ReturnsSuccessResponse()
        {
            var recommendedMenuDTO = RecommendedMenuTestData.RecommendedMenuDTOs().FirstOrDefault();
            var existingItem = RecommendedMenuTestData.FoodItems().FirstOrDefault();
            var newItem = RecommendedMenuTestData.FoodItems()[1];
            var existingRecommendedMenu = RecommendedMenuTestData.RecommendedMenuData().FirstOrDefault();

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(recommendedMenuDTO.OldItemName))
                .ReturnsAsync(existingItem);

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(recommendedMenuDTO.ItemName))
                .ReturnsAsync(newItem);

            _mockRecommendedMenuRepository.Setup(repo => repo.GetByItemId(existingItem.FoodItemId, recommendedMenuDTO.OldCategory, recommendedMenuDTO.RecommendationDate))
                .ReturnsAsync(existingRecommendedMenu);

            var result = await _recommendedMenuService.UpdateRecommendedMenu(recommendedMenuDTO);

            Assert.AreEqual("Update", result.Name);
            Assert.AreEqual("Updated successfully", result.Value);
        }

        [TestMethod]
        public async Task UpdateRecommendedMenu_InvalidMenu_ReturnsErrorResponse()
        {
            RecommendedMenuDTO recommendedMenuDTO = null;

            var result = await _recommendedMenuService.UpdateRecommendedMenu(recommendedMenuDTO);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("An error occurred: Invalid recommended menu details. Please provide valid data.", result.Value);
        }

        [TestMethod]
        public async Task UpdateRecommendedMenu_NonExistingItem_ReturnsErrorResponse()
        {
            var recommendedMenuDTO = RecommendedMenuTestData.RecommendedMenuDTOs().FirstOrDefault();
            FoodItem existingItem = null;

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(recommendedMenuDTO.OldItemName))
                .ReturnsAsync(existingItem);

            var result = await _recommendedMenuService.UpdateRecommendedMenu(recommendedMenuDTO);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("An error occurred: Existing item name not found.", result.Value);
        }

        [TestMethod]
        public async Task UpdateRecommendedMenu_NewItemNotFound_ReturnsErrorResponse()
        {
            var recommendedMenuDTO = RecommendedMenuTestData.RecommendedMenuDTOs().FirstOrDefault();
            var existingItem = RecommendedMenuTestData.FoodItems().FirstOrDefault();
            FoodItem newItem = null;

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(recommendedMenuDTO.OldItemName))
                .ReturnsAsync(existingItem);

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(recommendedMenuDTO.ItemName))
                .ReturnsAsync(newItem);

            var result = await _recommendedMenuService.UpdateRecommendedMenu(recommendedMenuDTO);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("An error occurred: New item name not found", result.Value);
        }

        #endregion UpdateRecommendedMenuMethod
    }
}
