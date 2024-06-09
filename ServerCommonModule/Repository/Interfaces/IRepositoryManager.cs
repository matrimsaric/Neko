using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Repository.Interfaces
{
    public interface IRepositoryManager<T>
    {
        void Clear();
        Task DeleteSingleItem(IDbConnection dbConnection, IDbTransaction transaction, T item);
        Task DeleteSingleItem(T item);
        Task InsertSingleItem(IDbConnection dbConnection, IDbTransaction transaction, T item);
        Task InsertSingleItem(T item);
        Task LoadCollection();
        Task LoadCollection(IDbConnection connection, IDbTransaction transaction, bool useReadUncommitted = false);
        Task SaveCollection();
        Task SaveCollection(IDbConnection dbConnection, DbTransaction sqlTransaction);
        Task UpdateSingleItem(IDbConnection dbConnection, IDbTransaction transaction, T item);
        Task UpdateSingleItem(T item);
    }
}
