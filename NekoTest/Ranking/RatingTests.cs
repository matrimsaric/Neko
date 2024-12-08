using RankingDomain.ControlModule;
using RankingDomain.Model;
using ServerCommonModule.Control.Procedures;
using ServerCommonModule.Control;
using ServerCommonModule.Database.Interfaces;
using ServerCommonModule.Database;
using System.Data;
using NuGet.Frameworks;

namespace NekoTest.Ranking
{
    [TestClass]
    public class RatingTests
    {
        private RatingManager _rankingManager = new RatingManager();
        private string TEST_NAME = "TESTRANK";

        [TestMethod]
        public void LoadFromIdTest()
        {
            // Note this will have to be done in order in test alonng with other 
            // wipeable test data

            Task<Rating> rank = _rankingManager.GetFromGuid(new Guid("6269941b-58ec-4f3a-bd4c-98b94fae3fc6"));

            if (rank == null)
            {
                Assert.Fail("Expecting a Rank to be returned when a valid Uuid is set");
            }
            else
            {
                Assert.IsTrue(rank.Result.RatingValue == 1500, " Expected rating is 1500 for this test");
            }

        }

        [TestMethod]
        public async Task CreateTestRank()
        {
            Rating testRank = new Rating();
            testRank.Id = Guid.NewGuid();
            testRank.Name = "TESTRANK";
            testRank.RatingValue = 1600;
            testRank.Deviation = 200;
            testRank.Volatility = 4.0m;

            // Act
            await _rankingManager.CreateRating(testRank);

            // Get
            Task<Rating> rank = _rankingManager.GetFromGuid(testRank.Id);

            // Assert
            Assert.AreEqual(testRank.Id, rank.Result.Id);
            Assert.AreEqual(testRank.Name, rank.Result.Name);
            Assert.AreEqual(testRank.RatingValue, rank.Result.RatingValue);
            Assert.AreEqual(testRank.Deviation, rank.Result.Deviation);
            Assert.AreEqual(testRank.Volatility, rank.Result.Volatility);
        }

        [TestMethod]
        public async Task TestTidy()
        {
            IEnvironmentalParameters envParms = new EnvironmentalParameters();
            envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";// TODO Note we need a much better single mechanism to do this for all classes
            envParms.DatabaseType = "PostgreSQL";
            IDbUtilityFactory dbUtilityFactory = new PgUtilityFactory(envParms, null);

            DirectAccess directAccess = new DirectAccess(dbUtilityFactory, envParms);

            string sql = "exec test_tidy";

            List<Tuple<string, string, SqlDbType>> dbParameters = new List<Tuple<string, string, SqlDbType>>();

            Tuple<string, string, SqlDbType> targetParam = new Tuple<string, string, SqlDbType>("test_name", "TESTRANK", SqlDbType.NVarChar);
            dbParameters.Add(targetParam);

            try
            {
                await directAccess.ExecuteNonQueryStandAloneProcedure(PROCEDURES.CLEAR_TEST_DATA, null, true, dbParameters);
            }
            catch (Exception ex)
            {
                Assert.Fail($" Test clean up threw an exception {ex.Message}");
            }

        }

        [TestMethod]
        public async Task DeleteRank()
        {
            Rating testRank = new Rating();
            testRank.Id = Guid.NewGuid();
            testRank.Name = "TESTRANK2";
            testRank.RatingValue = 1600;
            testRank.Deviation = 200;
            testRank.Volatility = 4.0m;

            // Act
            await _rankingManager.CreateRating(testRank);
            Task<Rating> rankInitial = _rankingManager.GetFromGuid(testRank.Id);
            Task<string> deleteResult = _rankingManager.DeleteRating(testRank);
            Task<Rating> rankDeleted = _rankingManager.GetFromGuid(testRank.Id);

            // Assert
            Assert.IsNotNull(rankInitial, "Create should have created a new user)");
            Assert.AreEqual("DONE", deleteResult.Result.ToString().ToUpper(), "Delete should respond with a DONE task if complete");
            Assert.IsNull(rankDeleted.Result, "Rank should have been removed and returned a null entry");

        }
    }
}