using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.Model
{
    public class ArchiveRankingCollection : ModelEntryCollection<ArchiveRank>
    {
        public ArchiveRankingCollection()
        : base(false)    //sorted = true
        {

        }

        public override ArchiveRank CreateItem()
        {
            return new ArchiveRank();
        }

        public override string GetBaseName()
        {
            return base.GetBaseName();
        }

        public override string GetBaseExternalId()
        {
            return base.GetBaseExternalId() + "Id";
        }

        public static implicit operator Task<object>(ArchiveRankingCollection v)
        {
            throw new NotImplementedException();
        }
    }
}
