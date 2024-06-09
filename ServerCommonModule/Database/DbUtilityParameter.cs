using ServerCommonModule.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database
{
    public class DbUtilityParameter : IDbUtilityParameter
    {
        public string ConnectionString { get; set; }

        public int DatabaseCommandTimeoutInSeconds { get; private set; } = 0;

        public HashSet<string> DatabaseErrorsMaskManagerErrorCodes { get; private set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public int DatabaseErrorsMaskManagerIntervalInMilliseconds { get; private set; } = 1000;

        public int DatabaseErrorsMaskManagerMaxRetryNumber { get; private set; } = 3;

        public void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void SetParameter(int databaseCommandTimeoutInSeconds, int databaseErrorsMaskManagerIntervalInMilliseconds, int databaseErrorsMaskManagerMaxRetryNumber, HashSet<string> databaseErrorsMaskManagerErrorCodes)
        {
            DatabaseCommandTimeoutInSeconds = databaseCommandTimeoutInSeconds;
            DatabaseErrorsMaskManagerIntervalInMilliseconds = databaseErrorsMaskManagerIntervalInMilliseconds;
            DatabaseErrorsMaskManagerMaxRetryNumber = databaseErrorsMaskManagerMaxRetryNumber;
            DatabaseErrorsMaskManagerErrorCodes = databaseErrorsMaskManagerErrorCodes;
        }
    }
}
