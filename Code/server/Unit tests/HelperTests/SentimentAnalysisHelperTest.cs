using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommendationEngineServer.Helpers;
using RecommendationEngineServer.Models.DTOs;
using ServerUnitTests.testData;

namespace ServerUnitTests.HelperTests
{
     [TestClass]
    public class SentimentAnlysisHelperTest
    {
        [TestMethod]
        public async Task AnalyzeSentiments_HasPositiveComments_ReturnsPositiveSentimentScore()
        {
            List<string> comments = HelperTestData.Comments().Take(2).ToList();
            UserData.RoleId = 1;

            var result = await SentimentAnlysisHelper.AnalyzeSentiments(comments, 4);

            Assert.AreEqual("3", result);
        }

        [TestMethod]
        public async Task AnalyzeSentiments_HasNegativeComments_ReturnsNegativeSentimentScore()
        {
            var comments = HelperTestData.Comments().Skip(4).Take(2).ToList();

            UserData.RoleId = 1;

            var result = await SentimentAnlysisHelper.AnalyzeSentiments(comments, 3);

            Assert.AreEqual("-3", result);
        }

        [TestMethod]
        public async Task AnalyzeSentiments_MixedComments_ReturnsNeutralSentimentScore()
        {
            // Arrange
            var comments = HelperTestData.Comments().Skip(2).Take(2).ToList();
            UserData.RoleId = 2;

            // Act
            var result = await SentimentAnlysisHelper.AnalyzeSentiments(comments, 3.5);

            // Assert
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public async Task AnalyzeSentiments_PositiveCOmments_ReturnsPositive()
        {
            List<string> comments = HelperTestData.Comments().Take(2).ToList();
            UserData.RoleId = 3;

            var result = await SentimentAnlysisHelper.AnalyzeSentiments(comments, 4);

            Assert.AreEqual("Positive", result);
        }

        [TestMethod]
        public async Task AnalyzeSentiments_NegativeCOmments_ReturnsNegative()
        {
            List<string> comments = HelperTestData.Comments().Skip(4).Take(2).ToList();
            UserData.RoleId = 3;

            var result = await SentimentAnlysisHelper.AnalyzeSentiments(comments, 3);

            Assert.AreEqual("Negative", result);
        }

        [TestMethod]
        public async Task AnalyzeSentiments_MixedCOmments_ReturnsNeutral()
        {
            List<string> comments = HelperTestData.Comments().Skip(2).Take(2).ToList();
            UserData.RoleId = 3;

            var result = await SentimentAnlysisHelper.AnalyzeSentiments(comments, 3.5);

            Assert.AreEqual("Neutral", result);
        }
    }
}