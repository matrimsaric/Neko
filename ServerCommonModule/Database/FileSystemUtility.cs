using ServerCommonModule.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database
{
    public class FileSystemUtility : IFileSystemUtility
    {
        public Task<IDataReader> ExecuteReader<T>(string location) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<string> GetFileLocation()
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertSingleRecord<T>(string location) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
