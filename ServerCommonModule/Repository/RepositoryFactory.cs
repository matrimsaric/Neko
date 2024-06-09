using ServerCommonModule.Repository.Interfaces;
using ServerCommonModule.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Repository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IDbUtilityFactory _dbUtilityFactory = null;
        private readonly IEnvironmentalParameters _environmentParameters = null;

        public RepositoryFactory(IDbUtilityFactory dbUtilityFactory, IEnvironmentalParameters environmentParameters)
        {
            _dbUtilityFactory = dbUtilityFactory;
            _environmentParameters = environmentParameters;
        }

        public IRepositoryManager<T> Get<T>(DataCollection<T> collection)
        {
            RepositoryManager<T> collectionManager = new RepositoryManager<T>(_dbUtilityFactory, _environmentParameters);
            collectionManager.SetCollection(collection);
            return collectionManager;
        }

    }
}
