using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Repository.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepositoryManager<T> Get<T>(DataCollection<T> collection);
    }
}
