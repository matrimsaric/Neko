using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.model;

namespace UserDomain.Model
{
    public class UserImageCollection : ModelEntryCollection<UserImage>
    {

        public UserImageCollection()
          : base(false)    //sorted = true
        {

        }

        public override UserImage CreateItem()
        {
            return new UserImage();
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
