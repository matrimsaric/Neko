using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.Model
{
    public class RatingCollection : ModelEntryCollection<Rating>
    {
        public RatingCollection()
        : base(false)    //sorted = true
        {

        }

        public override Rating CreateItem()
        {
            return new Rating();
        }

        public override string GetBaseName()
        {
            return base.GetBaseName();
        }

        public override string GetBaseExternalId()
        {
            return base.GetBaseExternalId() + "Id";
        }

        public static implicit operator Task<object>(RatingCollection v)
        {
            throw new NotImplementedException();
        }
    }
}
