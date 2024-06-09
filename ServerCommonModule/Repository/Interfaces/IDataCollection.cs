using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Repository.Interfaces
{
    public interface IDataCollection
    {
        int Count { get; }
        string TableName { get; }
        void Clear();
        void ToCollection(DataTable dataTable);
        DataTable ToDataTable();
        string ToString();
    }
}