using RankingDomain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.ControlModule.Interfaces
{
    public interface IArchiveRatingManager
    {
        public Task<string> CreateArchiveRank(Rating newRank, bool reload = true);

        public Task<string> UpdateArchiveRank(ArchiveRating newRank, bool reload = true);

        public Task<string> DeleteArchiveRank(ArchiveRating deleteRank, bool reload = true);

        public Task<string> SaveArchiveRank(ArchiveRating saveRank, bool reload = true);

        public Task<string> DeleteArchiveRankName(string test_name, bool reload = true);
    }
}
