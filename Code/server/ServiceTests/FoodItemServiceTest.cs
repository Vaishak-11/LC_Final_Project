using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;
using RecommendationEngineServer.Repositories.Interfaces;
using RecommendationEngineServer.Services;
using RecommendationEngineServer.Services.Interfaces;
using ServerUnitTests.testData;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ServerUnitTests.ServiceTests
{
    [TestClass]
    public class FoodItemServiceTest
    {
        #region Private Properties

        private FoodItemService _foodItemService;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<IFoodItemRepository> _mockFoodItemRepository;
        private Mock<IFeedbackRepository> _mockFeedbackRepository;
        private  Mock<IMapper> _mockMapper;
        private Mock<ILogger<FoodItemService>> _mockLogger;

        #endregion Private Properties

        #region Constructor

        [TestInitialize]
        public void Setup()
        {
            _mockFoodItemRepository = new Mock<IFoodItemRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<FoodItemService>>();


            _foodItemService = new FoodItemService(
                _mockFoodItemRepository.Object,
                _mockMapper.Object,
                _mockNotificationService.Object,
                _mockFeedbackRepository.Object,
                _mockLogger.Object);
        }

        #endregion Constructor

        #region Test Methods

        #region Add Method

        [TestMethod]
        public async Task Add_InvalidItem_ShouldReturnErrorResponse()
        {
            var foodItemDto = (FoodItemDTO) null;

            var result = await _foodItemService.Add(foodItemDto);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("Invalid item. Enter proper details.", result.Value);
        }

        [TestMethod]
        public async Task Add_ExistingItem_ShouldReturnErrorResponse()
        {
            var foodItemDto = FoodItemTestData.FoodItemDTO().FirstOrDefault();
            var existingItem = FoodItemTestData.FoodItems().FirstOrDefault();

            _mockFoodItemRepository.Setup(r => r.GetByItemName(foodItemDto.ItemName)).ReturnsAsync(existingItem); // Simulating item already exists

            var result = await _foodItemService.Add(foodItemDto);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("This item name already exists.", result.Value);
        }

        [TestMethod]
        public async Task Add_ValidItem_ShouldReturnSuccessResponse()
        {
            var foodItemDto = FoodItemTestData.FoodItemDTO().FirstOrDefault();

            _mockFoodItemRepository.Setup(r => r.GetByItemName(foodItemDto.ItemName)).ReturnsAsync((FoodItem)null);
            _mockFoodItemRepository.Setup(r => r.Add(It.IsAny<FoodItem>())).ReturnsAsync(1); // Simulating successful addition

            var result = await _foodItemService.Add(foodItemDto);

            Assert.AreEqual("AddItem", result.Name);
            Assert.AreEqual("Item added successfully.", result.Value);
        }

        #endregion Add Method

        #region Update Method

        [TestMethod]
        public async Task Update_InvalidItem_ShouldReturnErrorResponse()
        {
            var foodItemDto = (FoodItemDTO)null;

            var result = await _foodItemService.Update("dosa", foodItemDto, "y");

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("Invalid item. Enter proper details.", result.Value);
        }

        [TestMethod]
        public async Task Update_NonExistingItem_ShouldReturnErrorResponse()
        {
            var foodItemDto = FoodItemTestData.FoodItemDTO().FirstOrDefault();

            _mockFoodItemRepository.Setup(r => r.GetByItemName(foodItemDto.ItemName)).ReturnsAsync((FoodItem)null);

            var result = await _foodItemService.Update("poori", foodItemDto, "y");

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("Item not found.", result.Value);
        }

        [TestMethod]
        public async Task Update_ValidItem_ShouldReturnSuccessResponse()
        {
            var foodItemDto = FoodItemTestData.FoodItemDTO()[1];
            var existingItemOldName = FoodItemTestData.FoodItems().FirstOrDefault(); 
            var item = FoodItemTestData.FoodItems()[1];
            var updatedItem = new FoodItem
            {
                FoodItemId = existingItemOldName.FoodItemId,
                ItemName = foodItemDto.ItemName,
                Price = foodItemDto.Price,
                FoodCategory = foodItemDto.FoodCategory,
                IsAvailable = true 
            };

            _mockFoodItemRepository.Setup(r => r.GetByItemName("Dosa")).ReturnsAsync(existingItemOldName);
            _mockFoodItemRepository.Setup(r => r.GetByItemName(foodItemDto.ItemName)).ReturnsAsync((FoodItem)null); 
            _mockFoodItemRepository.Setup(r => r.Update(It.IsAny<FoodItem>())).Returns(Task.CompletedTask);
            _mockNotificationService.Setup(n => n.AddNotification(It.IsAny<Notification>())).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<FoodItemDTO, FoodItem>(foodItemDto))
                .Returns(updatedItem);

            var result = await _foodItemService.Update("Dosa", foodItemDto, "y");

            Assert.AreEqual("Update", result.Name);
            Assert.AreEqual("Updated succesfully", result.Value);
        }


        #endregion Update Method

        #region Delete Method
        [TestMethod]
        public async Task Delete_NonExistingItem_ShouldReturnErrorResponse()
        {
            var itemName = "poori";

            _mockFoodItemRepository.Setup(r => r.GetByItemName(itemName)).ReturnsAsync((FoodItem)null);

            var result = await _foodItemService.Delete(itemName);

            Assert.AreEqual("Error", result.Name);
            Assert.AreEqual("Item not found.", result.Value);
        }

        [TestMethod]
        public async Task Delete_ValidItem_ShouldReturnSuccessResponse()
        {
            var itemName = "Dosa";
            var existingItem = FoodItemTestData.FoodItems().FirstOrDefault();

            _mockFoodItemRepository.Setup(r => r.GetByItemName(itemName)).ReturnsAsync(existingItem);
            _mockFoodItemRepository.Setup(r => r.Delete(It.IsAny<int>()));
            _mockNotificationService.Setup(n => n.AddNotification(It.IsAny<Notification>())).ReturnsAsync(1);

            var result = await _foodItemService.Delete(itemName);

            Assert.AreEqual("Delete", result.Name);
            Assert.AreEqual("Deleted successfully", result.Value);
        }

        #endregion Delete Method

        #region GetList Method

        [TestMethod]
        public async Task GetList_ShouldReturnFoodItems()
        {
            var foodItems = FoodItemTestData.FoodItems();
            _mockFoodItemRepository.Setup(r => r.GetList(It.IsAny<Expression<Func<FoodItem, bool>>>())).ReturnsAsync(foodItems);
            _mockFeedbackRepository.Setup(r => r.GetList(It.IsAny<Expression<Func<Feedback, bool>>>())).ReturnsAsync(FoodItemTestData.Feedbacks());

            var result = await _foodItemService.GetList();

            Assert.IsNotNull(result);
            Assert.AreEqual("ItemList", result.Name);

            var resultItems = JsonSerializer.Deserialize<List<DisplayMenuDTO>>(result.Value.ToString());

            Assert.IsNotNull(resultItems);
            Assert.AreEqual(foodItems.Count, resultItems.Count);
        }

        [TestMethod]
        public async Task GetList_NoFoodItems_ShouldReturnEmptyList()
        {
            _mockFoodItemRepository.Setup(r => r.GetList(It.IsAny<Expression<Func<FoodItem, bool>>>())).ReturnsAsync((List<FoodItem>)null);

            var result = await _foodItemService.GetList();

            Assert.IsNotNull(result);
            Assert.AreEqual("ItemList", result.Name); 
        }

        #endregion GetList Method

        #endregion Test Methods
    }
}
