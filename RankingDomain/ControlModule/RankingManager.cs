using RankingDomain.ControlModule.Interfaces;
using RankingDomain.Model;
using ServerCommonModule.Database;
using ServerCommonModule.Database.Interfaces;
using ServerCommonModule.Repository;
using ServerCommonModule.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.ControlModule
{
    public class RankingManager : IRankingManager, IRankingGets
    {
        private IRepositoryFactory fact;
        private IRepositoryManager<Rank> rankManager;
        //private IDbUtility dbUtility;
        private IDbUtilityFactory dbUtilityFactory;
        private RankingCollection ranks = new RankingCollection();
        private ArchiveManager archiveManager = new ArchiveManager();

        public RankingManager()
        {
            IEnvironmentalParameters envParms = new EnvironmentalParameters();
            envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";
            envParms.DatabaseType = "PostgreSQL";
            dbUtilityFactory = new PgUtilityFactory(envParms, null);
            rankManager = new RepositoryManager<Rank>(dbUtilityFactory, envParms);

            fact = new RepositoryFactory(dbUtilityFactory, envParms);
        }
        #region Load Methods
        private async Task<RankingCollection> LoadCollection(bool reload)
        {
            if (reload || ranks?.Count == 0)
            {
                ranks = new RankingCollection();
                rankManager = fact.Get(ranks);
                await rankManager.LoadCollection();
            }

            if (ranks == null) ranks = new RankingCollection();

            return ranks;
        }

        #endregion Load Methods

        public async Task<string> CreateRank(Rank newRank, bool reload = true)
        {
            string status = System.String.Empty;
            RankingCollection liveRanks = await LoadCollection(reload);

            // When adding a new rank we want to archive any existing
            Rank existingRank = await GetFromGuid(newRank.Id);
            if (existingRank != null)
            {
               await archiveManager.CreateArchiveRank (existingRank);
            }

            // Then create the new Rank
            liveRanks.Add(newRank);
            await rankManager.InsertSingleItem(newRank);
            return status;
        }

        public async Task<string> DeleteRank(Rank deleteRank, bool reload = true)
        {
            RankingCollection liveRanks = await LoadCollection(reload);

            foreach (var rank in liveRanks.Where(x => x.Id == deleteRank.Id))
            {
                await rankManager.DeleteSingleItem(rank);
            }
            return "Done";
        }

        public async Task<Rank> GetFromGuid(Guid guid, bool reload = true)
        {
            RankingCollection ranks = await LoadCollection(reload);
            return ranks.FindById(guid);
        }

        public Task<string> SaveRank(Rank saveRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateRank(Rank newRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeleteRankName(string test_name, bool reload = true)
        {
            RankingCollection ranks = await LoadCollection(reload);

            foreach (var rank in ranks.Where(x => x.Name == test_name))
            {
                await rankManager.DeleteSingleItem(rank);
            }
            return "Done";
        }
    }
}
