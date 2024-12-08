using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingDomain.Model
{
    public class ArchiveRatingCollection : ModelEntryCollection<ArchiveRating>
    {
        public ArchiveRatingCollection()
        : base(false)    //sorted = true
        {

        }

        public override ArchiveRating CreateItem()
        {
            return new ArchiveRating();
        }

        public override string GetBaseName()
        {
            return base.GetBaseName();
        }

        public override string GetBaseExternalId()
        {
            return base.GetBaseExternalId() + "Id";
        }

        public static implicit operator Task<object>(ArchiveRatingCollection v)
        {
            throw new NotImplementedException();
        }
    }
}
