using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.model;

namespace UserDomain.Model
{
    public class ArchiveUserCollection : ModelEntryCollection<ArchiveUser>
    {

        public ArchiveUserCollection()
          : base(true)    //sorted = true
        {

        }

        public override ArchiveUser CreateItem()
        {
            return new ArchiveUser();
        }

        public override string GetBaseName()
        {
            return base.GetBaseName();
        }

        public override string GetBaseExternalId()
        {
            return base.GetBaseExternalId() + "Id";
        }
    }
}
