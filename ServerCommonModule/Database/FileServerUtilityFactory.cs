using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerCommonModule.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database
{
    public class PgUtilityFactory : IDbUtilityFactory
    {
        private readonly IEnvironmentalParameters _environmentParameters;
        private readonly IServiceProvider _serviceProvider;


        public PgUtilityFactory(IEnvironmentalParameters environmentParameters, IServiceProvider serviceProvider)
        {
            _environmentParameters = environmentParameters;
            _serviceProvider = serviceProvider;

        }

        public PgUtilityFactory(IOptionsMonitor<EnvironmentalParameters> envParams, IServiceProvider serviceProvider)
        {
            _environmentParameters = envParams.CurrentValue;
            _serviceProvider = serviceProvider;

        }

        public IDbUtility Get()
        {
            if (_serviceProvider != null)
            {
                IDbUtilityParameter dbUtilityParameter = _serviceProvider.GetRequiredService<IDbUtilityParameter>();

                IDbUtility dbUtility = new PgUtility(dbUtilityParameter, _environmentParameters);
                return dbUtility;
            }
            else
            {
                if (_environmentParameters.Database == Model.ConnectionType.POSTGRESS)
                {
                    IDbUtilityParameter dbUtilityParameter = new DbUtilityParameter();
                    dbUtilityParameter.ConnectionString = _environmentParameters.ConnectionString;


                    IDbUtility dbUtility = new PgUtility(dbUtilityParameter, _environmentParameters);
                    return dbUtility;
                }
            }
            return null;

        }
    }
}
