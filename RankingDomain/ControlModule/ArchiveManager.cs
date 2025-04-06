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
    public class ArchiveManager : IArchiveRatingManager, IArchiveRatingGets
    {
        private IRepositoryFactory fact;
        private IRepositoryManager<ArchiveRating> archiveManager;
        private IDbUtility dbUtility;
        private IDbUtilityFactory dbUtilityFactory;
        private ArchiveRatingCollection archiveRanks = new ArchiveRatingCollection();

        public ArchiveManager()
        {
            IEnvironmentalParameters envParms = new EnvironmentalParameters();
            envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";// TODO Note we need a much better single mechanism to do this for all classes
            envParms.DatabaseType = "PostgreSQL";
            dbUtilityFactory = new PgUtilityFactory(envParms, null);
            archiveManager = new RepositoryManager<ArchiveRating>(dbUtilityFactory, envParms);

            fact = new RepositoryFactory(dbUtilityFactory, envParms);
        }

        #region Load Methods
        private async Task<ArchiveRatingCollection> LoadCollection(bool reload)
        {
            if (reload || archiveRanks?.Count == 0)
            {
                archiveRanks = new ArchiveRatingCollection();
                archiveManager = fact.Get(archiveRanks);
                await archiveManager.LoadCollection();
            }

            if (archiveRanks == null) archiveRanks = new ArchiveRatingCollection();

            return archiveRanks;
        }

        #endregion Load Methods
        public async Task<string> CreateArchiveRank(Rating  newRank, bool reload = true)
        {
            ArchiveRatingCollection archiveRanks = await LoadCollection(reload);
            string status = String.Empty;

            // CHange - dont add archived date, just use modified then we have a history with no buggering about
            ArchiveRating archiveRank = new ArchiveRating();
            archiveRank.Id = newRank.Id;
            archiveRank.RatingValue = newRank.RatingValue;
            archiveRank.Deviation = newRank.Deviation;
            archiveRank.Volatility = newRank.Volatility;
            archiveRank.Name = newRank.Name;

            // Then create the new Rank
            archiveRanks.Add(archiveRank);
            await archiveManager.InsertSingleItem(archiveRank);

            return status;
        }

        public async Task<string> CreateConnectedArchiveRank(Rating newRank, Rating currentRating, Guid linkId, string reason, bool reload = true)
        {
            ArchiveRatingCollection archiveRanks = await LoadCollection(reload);
            string status = String.Empty;

            // CHange - dont add archived date, just use modified then we have a history with no buggering about
            ArchiveRating archiveRank = new ArchiveRating();
            archiveRank.Id = newRank.Id;
            archiveRank.UniqueIdentifier = linkId;
            archiveRank.Name = reason;
            archiveRank.RatingValue = newRank.RatingValue;
            archiveRank.Deviation = newRank.Deviation;
            archiveRank.Volatility = newRank.Volatility;
            archiveRank.Name = newRank.Name;

            // Then create the new Rank
            archiveRanks.Add(archiveRank);
            await archiveManager.InsertSingleItem(archiveRank);

            return status;
        }

        public Task<string> DeleteArchiveRank(ArchiveRating deleteRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public async  Task<ArchiveRating> GetFromGuid(Guid guid, bool reload = true)
        {
            ArchiveRatingCollection  ranks = await LoadCollection(reload);
            return ranks.FindById(guid);
        }

        public Task<string> SaveArchiveRank(ArchiveRating saveRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateArchiveRank(ArchiveRating newRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeleteArchiveRankName(string test_name, bool reload = true)
        {
            ArchiveRatingCollection archiveRanks = await LoadCollection(reload);

            foreach (var archive in archiveRanks.Where(x => x.Name == test_name))
            {
                await archiveManager.DeleteSingleItem(archive);
            }
            return "Done";
        }
    }
}
