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
        //private IRepositoryManager<ArchiveUser> archiveUserManager;
        private IDbUtility dbUtility;
        private IDbUtilityFactory dbUtilityFactory;
        private RankingCollection ranks = new RankingCollection();


        public RankingManager()
        {
            IEnvironmentalParameters envParms = new EnvironmentalParameters();
            envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";
            envParms.DatabaseType = "PostgreSQL";
            dbUtilityFactory = new PgUtilityFactory(envParms, null);

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

            if(ranks == null) ranks = new RankingCollection();

            return ranks;
        }
        #endregion Load Methods

        public Task<string> ArchiveRank(Rank newRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateRank(Rank newRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteRank(Rank deleteRank, bool reload = true)
        {
            throw new NotImplementedException();
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
    }
}
