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
            Rank testRank = new Rank();
            testRank.Id = Guid.NewGuid();
            testRank.Name = TEST_NAME;
            testRank.Rating = 1600;
            testRank.Deviation = 200;
            testRank.Volatility = 4.0m;

            // Act
            await _archiveManager.CreateArchiveRank(testRank);

            // Get
            Task<ArchiveRank> rank = _archiveManager.GetFromGuid(testRank.Id);

            // Assert
            Assert.AreEqual(testRank.Id, rank.Result.Id);
            Assert.AreEqual(testRank.Name, rank.Result.Name);
            Assert.AreEqual(testRank.Rating, rank.Result.Rating);
            Assert.AreEqual(testRank.Deviation, rank.Result.Deviation);
            Assert.AreEqual(testRank.Volatility, rank.Result.Volatility);
        }


    }
}