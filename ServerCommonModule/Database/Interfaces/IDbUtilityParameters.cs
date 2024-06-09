using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database.Interfaces
{
    public interface IDbUtilityParameter
    {
        string ConnectionString { get; set; }
        int DatabaseCommandTimeoutInSeconds { get; }
        HashSet<string> DatabaseErrorsMaskManagerErrorCodes { get; }

        int DatabaseErrorsMaskManagerIntervalInMilliseconds { get; }
        int DatabaseErrorsMaskManagerMaxRetryNumber { get; }

        void SetConnectionString(string connectionString);

        void SetParameter(int databaseCommandTimeoutInSeconds, int databaseErrorsMaskManagerIntervalInMilliseconds,
            int databaseErrorsMaskManagerMaxRetryNumber, HashSet<string> databaseErrorsMaskManagerErrorCodes);
    }
}
