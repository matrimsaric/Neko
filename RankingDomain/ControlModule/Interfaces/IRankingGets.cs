using RankingDomain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.ControlModule.Interfaces
{
    public interface IRankingGets
    {
        public Task<Rank> GetFromGuid(Guid guid, bool reload = true);
    }
}
