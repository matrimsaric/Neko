using RankingDomain.ControlModule;
using RankingDomain.Model;

namespace NekoTest
{
    [TestClass]
    public class RankingTests
    {
        [TestMethod]
        public void LoadFromIdTest()
        {
            // Note this will have to be done in order in test alonng with other 
            // wipeable test data
            RankingManager? rankingManager = new RankingManager();
            Task<Rank> rank = rankingManager.GetFromGuid(new Guid("6269941b-58ec-4f3a-bd4c-98b94fae3fc6"));

            if (rank == null)
            {
                Assert.Fail("Expecting a Rank to be returned when a valid Uuid is set");
            }
            else
            {
                Assert.IsTrue(rank.Result.Rating == 1500, " Expected rating is 1500 for this test");
            }

        }

        [TestMethod]
        public async Task CreateTestRank()
        {
            RankingManager? rankingManager = new RankingManager();
            Rank testRank = new Rank();
            testRank.Id = Guid.NewGuid();
            testRank.Name = "TESTRANK";
            testRank.Rating = 1600;
            testRank.Deviation = 200;
            testRank.Volatility = 4.0m;
            
            // Act
            await rankingManager.CreateRank(testRank);

            // Get
            Task<Rank> rank =  rankingManager.GetFromGuid(testRank.Id);

            // Assert
            Assert.AreEqual(testRank.Id, rank.Result.Id);
            Assert.AreEqual(testRank.Name, rank.Result.Name);
            Assert.AreEqual(testRank.Rating, rank.Result.Rating);
            Assert.AreEqual(testRank.Deviation, rank.Result.Deviation);
            Assert.AreEqual(testRank.Volatility, rank.Result.Volatility);
        }
    }
}