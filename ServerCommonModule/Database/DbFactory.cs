using ServerCommonModule.Database.Interfaces;
using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database
{
    internal class DbFactory
    {
        internal IDbUtility GetDatabaseAccess(ConnectionType connectionType, string connectionString)
        {
            IEnvironmentalParameters parameters = new EnvironmentalParameters();
            parameters.ConnectionString = connectionString;

            DbUtilityParameter dbParams = new()
            {
                ConnectionString = connectionString
            };
            
            switch (connectionType)
            {
                case ConnectionType.POSTGRESS:
                default:
                    return new PgUtility(dbParams, parameters);
            }


        }
    }
}
