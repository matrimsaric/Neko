using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database.Interfaces
{
    public interface IDbUtility
    {
        int DatabaseCommandTimeoutInSeconds { get; }
        int DatabaseErrorsMaskManagerQueryInterval { get; }
        int DatabaseErrorsMaskManagerMaxRetries { get; }

        Task<IDbConnection> GetConnection();

        IDbConnection GetBaseConnection();

        Task<int> ExecuteNonQuery(IDbConnection connection, string commandText, params IDataParameter[] parameters);

        Task<int> ExecuteNonQuery(IDbTransaction transaction, string commandText, params IDataParameter[] parameters);

        IDataParameter CreateSqlParameter(string parameterName, SqlDbType sqlDbType, object parameterValue);

        Task<IDataReader> ExecuteReader(IDbConnection connection, string commandText, params IDataParameter[] parameters);

        Task<IDataReader> ExecuteReader(IDbTransaction transaction, string commandText, params IDataParameter[] parameters);

        Task<T> ExecuteScalar<T>(IDbConnection connection, string commandText, params IDataParameter[] parameters);

        Task<T> ExecuteScalar<T>(IDbTransaction transaction, string commandText, params IDataParameter[] parameters);



    }
}
