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

            // TODO Create a new ArchiveRank from the existing Rank adjust the dates then save (then add test)
            // I think we need an extra id column to match that is different from the primary key standard id
            // that way we can collect multiples without tying ourselves in knots
            ArchiveRank archiveRank = new ArchiveRank();
            archiveRank.Id = newRank.Id;
            archiveRank.Rating = newRank.Rating;
            archiveRank.Deviation = newRank.Deviation;
            archiveRank.Volatility = newRank.Volatility;
            archiveRank.Name = newRank.Name;
            //archiveRank.ArchivedDate = newRank.ModifiedDate;// Also an issue we need to access the modified date in the model, otherwise the field is essentially just a time stamp, if it is then the rating need adjusting to add an actual date for the rating
            // which might make more sense

            // Then create the new Rank
            archiveRanks.Add(archiveRank);
            await archiveManager.InsertSingleItem(archiveRank);

            // Double action when we create we also generate a historical archive record with a link.. this will initially have no archive date.
            // Note that the table key is the user id and the modified date
            return status;
        }

        public Task<string> DeleteArchiveRank(ArchiveRank deleteRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<ArchiveRank> GetFromGuid(Guid guid, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveArchiveRank(ArchiveRank saveRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateArchiveRank(ArchiveRank newRank, bool reload = true)
        {
            throw new NotImplementedException();
        }
    }
}
