using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.Model
{
    public class RankingCollection : ModelEntryCollection<Rank>
    {
        public RankingCollection()
        : base(true)    //sorted = true
        {

        }

        public override Rank CreateItem()
        {
            return new Rank();
        }

        public override string GetBaseName()
        {
            return base.GetBaseName();
        }

        public override string GetBaseExternalId()
        {
            return base.GetBaseExternalId() + "Id";
        }

        public static implicit operator Task<object>(RankingCollection v)
        {
            throw new NotImplementedException();
        }
    }
}
