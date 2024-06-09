using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database.Interfaces
{
    public interface IPgUtility : IDbUtility
    {
        NpgsqlDbType GetNpgsqlDbType(SqlDbType sqlDbType);
    }
}
