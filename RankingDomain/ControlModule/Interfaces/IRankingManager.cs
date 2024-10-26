using RankingDomain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.ControlModule.Interfaces
{
    public interface IRankingManager
    {
        public Task<string> CreateRank(Rank newRank, bool reload = true);

        public Task<string> ArchiveRank(Rank newRank, bool reload = true);

        public Task<string> UpdateRank(Rank newRank, bool reload = true);

        public Task<string> DeleteRank(Rank deleteRank, bool reload = true);

        public Task<string> SaveRank(Rank saveRank, bool reload = true);
    }
}
