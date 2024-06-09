using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDomain.Properties
{
    public enum USER_STATUS
    {
        UNAUTHENTICATED = 0,
        ACTIVE = 1,
        INACTIVE = 2,
        ONHOLD = 3,
        SUSPENDED = 4,
        BANNED = 5,
        ARCHIVED = 6

    }
}
