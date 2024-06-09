using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database.Interfaces
{
    public interface IEnvironmentalParameters
    {
        string ConnectionString { get; set; }

        int ServerSessionTimeoutInSeconds { get; }
        bool ShowVerboseErrors { get; }

        ConnectionType Database { get; }

        string DatabaseType { get; set; }
    }
}
