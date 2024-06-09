using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.model;

namespace UserDomain.Model
{
    public class PrimeUserCollection : ModelEntryCollection<PrimeUser>
    {

        public PrimeUserCollection()
          : base(true)    //sorted = true
        {

        }

        public override PrimeUser CreateItem()
        {
            return new PrimeUser();
        }

        public override string GetBaseName()
        {
            return base.GetBaseName();
        }

        public override string GetBaseExternalId()
        {
            return base.GetBaseExternalId() + "Id";
        }

        public static implicit operator Task<object>(PrimeUserCollection v)
        {
            throw new NotImplementedException();
        }
    }
}
