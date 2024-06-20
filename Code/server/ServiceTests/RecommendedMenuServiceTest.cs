using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Models.Enums;
using RecommendationEngineServer.Profiles;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Services.Interfaces;
using ServerUnitTests.testData;

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

        [TestInitialize]
        public void Initialize()
        {
            _mockRecommendedMenuRepository = new Mock<IRecommendedMenuRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockFoodItemRepository = new Mock<IFoodItemRepository>();
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();
            _mockLogger = new Mock<ILogger<RecommendedMenuService>>();

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
    }


}
