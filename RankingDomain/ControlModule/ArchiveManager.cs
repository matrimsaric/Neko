using RankingDomain.ControlModule.Interfaces;
using RankingDomain.Model;
using ServerCommonModule.Database.Interfaces;
using ServerCommonModule.Database;
using ServerCommonModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommonModule.Repository.Interfaces;

namespace RankingDomain.ControlModule
{
    public class ArchiveManager : IArchiveRankingManager, IArchiveRankingGets
    {
        private IRepositoryFactory fact;
        private IRepositoryManager<ArchiveRank> archiveManager;
        private IDbUtility dbUtility;
        private IDbUtilityFactory dbUtilityFactory;
        private ArchiveRankingCollection archiveRanks = new ArchiveRankingCollection();

        public ArchiveManager()
        {
            IEnvironmentalParameters envParms = new EnvironmentalParameters();
            envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";// TODO Note we need a much better single mechanism to do this for all classes
            envParms.DatabaseType = "PostgreSQL";
            dbUtilityFactory = new PgUtilityFactory(envParms, null);
            archiveManager = new RepositoryManager<ArchiveRank>(dbUtilityFactory, envParms);

            fact = new RepositoryFactory(dbUtilityFactory, envParms);
        }

        #region Load Methods
        private async Task<ArchiveRankingCollection> LoadCollection(bool reload)
        {
            if (reload || archiveRanks?.Count == 0)
            {
                archiveRanks = new ArchiveRankingCollection();
                archiveManager = fact.Get(archiveRanks);
                await archiveManager.LoadCollection();
            }

            if (archiveRanks == null) archiveRanks = new ArchiveRankingCollection();

            return archiveRanks;
        }

        #endregion Load Methods
        public async Task<string> CreateArchiveRank(Rank  newRank, bool reload = true)
        {
            ArchiveRankingCollection archiveRanks = await LoadCollection(reload);
            string status = String.Empty;

            // CHange - dont add archived date, just use modified then we have a history with no buggering about
            ArchiveRank archiveRank = new ArchiveRank();
            archiveRank.Id = newRank.Id;
            archiveRank.Rating = newRank.Rating;
            archiveRank.Deviation = newRank.Deviation;
            archiveRank.Volatility = newRank.Volatility;
            archiveRank.Name = newRank.Name;

            // Then create the new Rank
            archiveRanks.Add(archiveRank);
            await archiveManager.InsertSingleItem(archiveRank);

            return status;
        }

        public Task<string> DeleteArchiveRank(ArchiveRank deleteRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public async  Task<ArchiveRank> GetFromGuid(Guid guid, bool reload = true)
        {
            ArchiveRankingCollection  ranks = await LoadCollection(reload);
            return ranks.FindById(guid);
        }

        public Task<string> SaveArchiveRank(ArchiveRank saveRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateArchiveRank(ArchiveRank newRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeleteArchiveRankName(string test_name, bool reload = true)
        {
            ArchiveRankingCollection archiveRanks = await LoadCollection(reload);

            foreach (var archive in archiveRanks.Where(x => x.Name == test_name))
            {
                await archiveManager.DeleteSingleItem(archive);
            }
            return "Done";
        }
    }
}
