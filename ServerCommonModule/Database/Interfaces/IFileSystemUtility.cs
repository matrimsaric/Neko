using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database.Interfaces
{
    public interface IFileSystemUtility
    {
        Task<string> GetFileLocation();


        Task<int> InsertSingleRecord<T>(string location) where T : class;

        Task<IDataReader> ExecuteReader<T>(string location) where T : class;


    }
}
