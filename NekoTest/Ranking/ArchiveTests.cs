using RankingDomain.ControlModule;
using RankingDomain.Model;

namespace NekoTest.Ranking
{
    [TestClass]
    public class ArchiveRankingTests
    {
        private ArchiveManager _archiveManager = new ArchiveManager();
        private string TEST_NAME = "TESTRANK";


        [TestMethod]
        public async Task CreateTestRank()
        {
            Rating testRank = new Rating();
            testRank.Id = Guid.NewGuid();
            testRank.Name = TEST_NAME;
            testRank.RatingValue = 1600;
            testRank.Deviation = 200;
            testRank.Volatility = 4.0m;

            // Act
            await _archiveManager.CreateArchiveRank(testRank);

            // Get
            Task<ArchiveRating> rank = _archiveManager.GetFromGuid(testRank.Id);

            // Assert
            Assert.AreEqual(testRank.Id, rank.Result.Id);
            Assert.AreEqual(testRank.Name, rank.Result.Name);
            Assert.AreEqual(testRank.RatingValue, rank.Result.RatingValue);
            Assert.AreEqual(testRank.Deviation, rank.Result.Deviation);
            Assert.AreEqual(testRank.Volatility, rank.Result.Volatility);
        }


    }
}