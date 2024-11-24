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
    public class RankingTests
    {
        private RankingManager _rankingManager = new RankingManager();
        private string TEST_NAME = "TESTRANK";

        [TestMethod]
        public void LoadFromIdTest()
        {
            // Note this will have to be done in order in test alonng with other 
            // wipeable test data
            
            Task<Rank> rank = _rankingManager.GetFromGuid(new Guid("6269941b-58ec-4f3a-bd4c-98b94fae3fc6"));

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
            Rank testRank = new Rank();
            testRank.Id = Guid.NewGuid();
            testRank.Name = "TESTRANK";
            testRank.Rating = 1600;
            testRank.Deviation = 200;
            testRank.Volatility = 4.0m;

            // Act
            await _rankingManager.CreateRank(testRank);

            // Get
            Task<Rank> rank = _rankingManager.GetFromGuid(testRank.Id);

            // Assert
            Assert.AreEqual(testRank.Id, rank.Result.Id);
            Assert.AreEqual(testRank.Name, rank.Result.Name);
            Assert.AreEqual(testRank.Rating, rank.Result.Rating);
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
            catch(Exception ex)
            {
                Assert.Fail($" Test clean up threw an exception {ex.Message}");
            }
            
        }


    }
}