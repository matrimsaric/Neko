using RankingDomain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.ControlModule.Interfaces
{
    public interface IRatingManager
    {
        public Task<string> CreateRating(Rating newRank, bool reload = true);

        public Task<string> UpdateRating(Rating newRank, Guid linkId, string sReason, bool reload = true);

        public Task<string> DeleteRating(Rating deleteRank, bool reload = true);

        public Task<string> SaveRating(Rating saveRank, bool reload = true);

        public Task<string> DeleteRatingName(string test_name, bool reload = true);
    }
}
