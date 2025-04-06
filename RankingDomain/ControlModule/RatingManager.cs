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
    public class RatingManager : IRatingManager, IRatingGets
    {
        private IRepositoryFactory fact;
        private IRepositoryManager<Rating> rankManager;
        //private IDbUtility dbUtility;
        private IDbUtilityFactory dbUtilityFactory;
        private RatingCollection ranks = new RatingCollection();
        private ArchiveManager archiveManager = new ArchiveManager();

        public RatingManager()
        {
            IEnvironmentalParameters envParms = new EnvironmentalParameters();
            envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";
            envParms.DatabaseType = "PostgreSQL";
            dbUtilityFactory = new PgUtilityFactory(envParms, null);
            rankManager = new RepositoryManager<Rating>(dbUtilityFactory, envParms);

            fact = new RepositoryFactory(dbUtilityFactory, envParms);
        }
        #region Load Methods
        private async Task<RatingCollection> LoadCollection(bool reload)
        {
            if (reload || ranks?.Count == 0)
            {
                ranks = new RatingCollection();
                rankManager = fact.Get(ranks);
                await rankManager.LoadCollection();
            }

            if (ranks == null) ranks = new RatingCollection();

            return ranks;
        }

        #endregion Load Methods

        public async Task<string> CreateRating(Rating newRank, bool reload = true)
        {
            string status = System.String.Empty;
            RatingCollection liveRanks = await LoadCollection(reload);

            // When adding a new rank we want to archive any existing
            Rating existingRank = await GetFromGuid(newRank.Id);
            if (existingRank != null)
            {
               await archiveManager.CreateArchiveRank (existingRank);
            }

            // Then create the new Rank
            liveRanks.Add(newRank);
            await rankManager.InsertSingleItem(newRank);
            return status;
        }

        public async Task<string> DeleteRating(Rating deleteRank, bool reload = true)
        {
            RatingCollection liveRanks = await LoadCollection(reload);

            foreach (var rank in liveRanks.Where(x => x.Id == deleteRank.Id))
            {
                await rankManager.DeleteSingleItem(rank);
            }
            return "Done";
        }

        public async Task<Rating> GetFromGuid(Guid guid, bool reload = true)
        {
            RatingCollection ranks = await LoadCollection(reload);
            return ranks.FindById(guid);
        }

        public Task<string> SaveRating(Rating saveRank, bool reload = true)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateRating(Rating newRank, Guid linkId, string sReason, bool reload = true)
        {
            // first up get current rating as we want to archive
            Task<Rating> currentRating = GetFromGuid(newRank.Id);
            bool runCreate = false;

            if(currentRating.Result == null)
            {
                // run create instead
                runCreate = true;
              
            }

            if (!runCreate)
            {
                string archiveCurrent = await archiveManager.CreateConnectedArchiveRank(newRank, currentRating.Result, linkId, sReason, reload);

                if (!String.IsNullOrEmpty(archiveCurrent))
                {
                    return "Cannot update rating as archiving process failed: " + archiveCurrent;
                }
            }
           

            // need to update existing rating TODO
            if (runCreate)
            {
                await CreateRating(newRank);
                return "created new rating, rating does not exist";
            }
            else
            {
                await rankManager.UpdateSingleItem(newRank);
            }

                return "done";


            
        }

        public async Task<string> DeleteRatingName(string test_name, bool reload = true)
        {
            RatingCollection ranks = await LoadCollection(reload);

            foreach (var rank in ranks.Where(x => x.Name == test_name))
            {
                await rankManager.DeleteSingleItem(rank);
            }
            return "Done";
        }
    }
}
