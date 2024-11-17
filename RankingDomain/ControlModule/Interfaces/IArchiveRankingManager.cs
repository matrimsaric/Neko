using RankingDomain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.ControlModule.Interfaces
{
    public interface IArchiveRankingManager
    {
        public Task<string> CreateArchiveRank(Rank newRank, bool reload = true);

        public Task<string> UpdateArchiveRank(ArchiveRank newRank, bool reload = true);

        public Task<string> DeleteArchiveRank(ArchiveRank deleteRank, bool reload = true);

        public Task<string> SaveArchiveRank(ArchiveRank saveRank, bool reload = true);

        public Task<string> DeleteArchiveRankName(string test_name, bool reload = true);
    }
}
