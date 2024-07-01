using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Services.Interfaces;
using ServerUnitTests.testData;
using System.Linq.Expressions;
using System.Text.Json;

namespace ServerUnitTests.ServiceTests
{
    [TestClass]
    public class FeedbackServiceTest
    {
        #region Private Properties

        private FeedbackService _feedbackService;
        private Mock<IFeedbackRepository> _mockFeedbackRepository;
        private Mock<IFoodItemRepository> _mockFoodItemRepository;
        private Mock<IOrderItemRepository> _mockOrderItemRepository;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<FeedbackService>> _mockLogger;

        #endregion Private Properties

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();
            _mockFoodItemRepository = new Mock<IFoodItemRepository>();
            _mockOrderItemRepository = new Mock<IOrderItemRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<FeedbackService>>();

            _feedbackService = new FeedbackService(
                                   _mockFeedbackRepository.Object,
                                    _mockFoodItemRepository.Object,
                                    _mockMapper.Object,
                                    _mockOrderItemRepository.Object,
                                    _mockLogger.Object,
                                    _mockNotificationService.Object);
                                }

        #endregion Constructor

        #region Test Methods

        #region AddFeedback Method

        [TestMethod]
        public async Task AddFeedback_InvalidFeedback_ShouldReturnErrorResponse()
        {
            var feedbackDto = (FeedbackDTO)null;

            var result = await _feedbackService.AddFeedback(feedbackDto);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("Invalid feedback details. Enter proper details.", result.Value);
        }

        [TestMethod]
        public async Task AddFeedback_ValidFeedback_ReturnsSuccessResponse()
        {
            var feedbackDTO = FeedbackServiceTestData.FeedbackDTOs().FirstOrDefault();
            var feedback = FeedbackServiceTestData.Feedbacks().FirstOrDefault();

            var foodItem = FeedbackServiceTestData.FoodItems().FirstOrDefault();

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(feedbackDTO.ItemName))
                .ReturnsAsync(foodItem);

            _mockOrderItemRepository.Setup(repo => repo.OrderItemExists(It.IsAny<Expression<Func<OrderItem, bool>>>()))
                .ReturnsAsync(true);

            _mockMapper.Setup(mapper => mapper.Map<FeedbackDTO, Feedback>(feedbackDTO))
                .Returns(feedback);

            _mockFeedbackRepository.Setup(repo => repo.Add(It.IsAny<Feedback>()))
                .ReturnsAsync(1); 

            var result = await _feedbackService.AddFeedback(feedbackDTO);

            Assert.AreEqual("AddFeedback", result.Name);
            Assert.AreEqual("Thank you for your feedback.", result.Value);
        }

        #endregion AddFeedback Method

        #region GetFeedbacks Method

        [TestMethod]
        public async Task GetFeedbacks_ValidItemName_ReturnsFeedbackList()
        {
            var itemName = "Dosa";
            var foodItem = FeedbackServiceTestData.FoodItems().FirstOrDefault();
            var feedbackList = FeedbackServiceTestData.Feedbacks().Take(1).ToList();
            var displayFeedbacks = FeedbackServiceTestData.DisplayFeedbackDTOs().Take(1).ToList();

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(itemName))
                .ReturnsAsync(foodItem);

            _mockFeedbackRepository.Setup(repo => repo.GetList(It.IsAny<Expression<Func<Feedback, bool>>>()))
                .ReturnsAsync(feedbackList);

            _mockMapper.Setup(mapper => mapper.Map<List<Feedback>, List<DisplayFeedbackDTO>>(feedbackList))
                .Returns(displayFeedbacks);

            var result = await _feedbackService.GetFeedbacks(itemName);

            Assert.AreEqual("Feedback List", result.Name);
            Assert.IsNotNull(result.Value);

            var feedbackDtoList = JsonSerializer.Deserialize<List<DisplayFeedbackDTO>>(result.Value.ToString());
            Assert.AreEqual(1, feedbackDtoList.Count);
        }

        [TestMethod]
        public async Task GetFeedbacks_WhenThereIsNpFeedbacks_ReturnsEmptyFeedbackList()
        {
            var itemName = "Dosa";
            var foodItem = FeedbackServiceTestData.FoodItems().FirstOrDefault();
            var feedbackList = new List<Feedback>();
            var displayFeedbacks = new List<DisplayFeedbackDTO>();

            _mockFoodItemRepository.Setup(repo => repo.GetByItemName(itemName))
                .ReturnsAsync(foodItem);

            _mockFeedbackRepository.Setup(repo => repo.GetList(It.IsAny<Expression<Func<Feedback, bool>>>()))
                .ReturnsAsync(feedbackList);

            _mockMapper.Setup(mapper => mapper.Map<List<Feedback>, List<DisplayFeedbackDTO>>(feedbackList))
                .Returns(displayFeedbacks);

            var result = await _feedbackService.GetFeedbacks(itemName);

            Assert.AreEqual("Feedback List", result.Name);
            Assert.IsNotNull(result.Value);

            Assert.AreEqual("No feedbacks available for this item.", result.Value.ToString());
        }

        #endregion GetFeedbacks Method

        #region GetDetailedFeedback Method

        [TestMethod]
        public async Task GetDetailedFeedback_ValidItemName_ReturnsDetailedFeedbackList()
        {
            var itemName = "Dosa";
            var feedbackList = FeedbackServiceTestData.Feedbacks().Skip(1).Take(1).ToList();

            _mockFeedbackRepository.Setup(repo => repo.GetList(It.IsAny<Expression<Func<Feedback, bool>>>()))
                .ReturnsAsync(feedbackList);

            var result = await _feedbackService.GetDetailedFeedback(itemName);

            Assert.AreEqual("DetailedFeedback", result.Name);
            Assert.IsNotNull(result.Value);

            var feedbackComments = JsonSerializer.Deserialize<List<string>>(result.Value.ToString());
            Assert.AreEqual(1, feedbackComments.Count);
        }

        #endregion GetDetailedFeedback Method

        #endregion Test Methods 
    }
}
