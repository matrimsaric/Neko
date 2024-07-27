using Npgsql;
using ServerCommonModule.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database
{
    public abstract class DbUtility(IDbUtilityParameter dbUtilityParameter, IEnvironmentalParameters environmentalParameters) : IDbUtility
    {

        protected string _server = String.Empty;
        protected string _database = String.Empty;
        protected string _commandText = String.Empty;
        protected IEnumerable<ParameterInfo> _parameterValues;

        public int DatabaseCommandTimeoutInSeconds { get { return _dbUtilityParameter.DatabaseCommandTimeoutInSeconds; } }
        public int DatabaseErrorsMaskManagerMaxRetries { get { return _dbUtilityParameter.DatabaseErrorsMaskManagerMaxRetryNumber; } }
        public int DatabaseErrorsMaskManagerQueryInterval { get { return _dbUtilityParameter.DatabaseErrorsMaskManagerIntervalInMilliseconds; } }

        public HashSet<string> DatabaseErrorsMaskManagerErrorCodes { get { return _dbUtilityParameter.DatabaseErrorsMaskManagerErrorCodes; } }

        protected abstract bool ContainsAnErrorToCheck(DbException dbException);
        private readonly IDbUtilityParameter _dbUtilityParameter = dbUtilityParameter;
        private readonly IEnvironmentalParameters _environmentalParameters = environmentalParameters;

        private enum ExecuteMode
        {
            ExecuteNonQuery,
            ExecuteScalar,
            ExecuteReader
        }

        public abstract IDataParameter CreateSqlParameter(string parameterName, SqlDbType sqlDbType, object parameterValue);

        protected abstract DbCommand DbCommand(IDbConnection connection, string commandText, params IDataParameter[] parameters);

        protected abstract Task<IDbConnection> GetConnection(string connectionString);

        public async Task<IDbConnection> GetConnection()
        {
            string connectionString = _dbUtilityParameter.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _environmentalParameters.ConnectionString;
            }

            return await GetConnection(connectionString);
        }

        public IDbConnection GetBaseConnection()
        {
            string connectionString = _dbUtilityParameter.ConnectionString;

            Debug.Assert(string.IsNullOrEmpty(connectionString) == false);
            DbConnection connection = new NpgsqlConnection(connectionString);

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            return connection;
        }

        private async Task<int> DbCommandExecute(DbCommand dbCommand)
        {
            int rowsAffected = 0;

            for (int i = 1; i < DatabaseErrorsMaskManagerMaxRetries; i++)
            {
                (bool result, int rowsAffected, object singleResult, DbDataReader) act = await TryExecuteAsync(dbCommand, ExecuteMode.ExecuteNonQuery, i < DatabaseErrorsMaskManagerMaxRetries);
                rowsAffected = act.rowsAffected;

                if (act.result)
                    break;

                await Task.Delay(DatabaseErrorsMaskManagerQueryInterval);
            }

            return rowsAffected;
        }

        private async Task<(bool result, int rowsAffected, object singleResult, DbDataReader dataReader)> TryExecuteAsync(DbCommand dbCommand, ExecuteMode executeMode, bool tryAgain)
        {
            bool result = true;
            int rowsAffected = 0;
            object singleResult = null;
            DbDataReader dataReader = null;
            try
            {
                switch (executeMode)
                {
                    case ExecuteMode.ExecuteNonQuery:
                        rowsAffected = dbCommand.ExecuteNonQuery();
                        break;
                    case ExecuteMode.ExecuteScalar:
                        singleResult = dbCommand.ExecuteScalar();
                        break;
                    case ExecuteMode.ExecuteReader:
                        dataReader = dbCommand.ExecuteReader();
                        break;
                }
                return (result, rowsAffected, singleResult, dataReader);
            }
            catch (DbException dbException)
            {
                if (IsExceptionMasked(tryAgain, dbException))
                {
                    result = false;
                    return (result, rowsAffected, singleResult, dataReader);
                }


                throw;
            }

        }

        public bool IsExceptionMasked(bool canTryAgain, DbException dbException)
        {
            if (ContainsAnErrorToCheck(dbException))
            {
                if (canTryAgain)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<int> ExecuteNonQuery(IDbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return await ExecuteNonQuery(connection, null, commandText, parameters);
        }

        public async Task<int> ExecuteNonQuery(IDbTransaction transaction, string commandText, params IDataParameter[] parameters)
        {
            return await ExecuteNonQuery(transaction.Connection, transaction, commandText, parameters);
        }

        private async Task<int> ExecuteNonQuery(IDbConnection connection, IDbTransaction transaction, string commandText, params IDataParameter[] parameters)
        {
            int rowsAffected = 0;

            try
            {
                using (DbCommand dbCommand = DbCommand(connection, commandText, parameters))
                {
                    if (transaction != null)
                    {
                        DbTransaction dbTransaction = transaction as DbTransaction;
                        dbCommand.Transaction = dbTransaction;
                    }

                    rowsAffected = await DbCommandExecute(dbCommand);
                }
                return rowsAffected;
            }
            finally
            {
            }
        }

        public async Task<IDataReader> ExecuteReader(IDbConnection connection, string commandText, IDataParameter[] parameters)
        {
            return await ExecuteReader(connection, null, commandText, parameters);
        }

        public async Task<IDataReader> ExecuteReader(IDbTransaction transaction, string commandText, IDataParameter[] parameters)
        {
            return await ExecuteReader(transaction.Connection, transaction, commandText, parameters);
        }

        public async Task<IDataReader> ExecuteReader(IDbConnection connection, IDbTransaction transaction, string commandText, params IDataParameter[] parameters)
        {
            try
            {
                using DbCommand dbCommand = DbCommand(connection, commandText, parameters);
                if (transaction != null)
                {
                    DbTransaction dbTransaction = transaction as DbTransaction;
                    dbCommand.Transaction = dbTransaction;
                }

                return await ExecuteReader(dbCommand, parameters);

            }
            finally
            {
            }
        }

        private async Task<IDataReader> ExecuteReader(DbCommand dbCommand, params IDataParameter[] parameters)
        {
            DbDataReader dataReader = null ;


            for (int i = 1; i < DatabaseErrorsMaskManagerMaxRetries; i++)
            {
                (bool result, int rowsAffected, object singleResult, DbDataReader dataReader) act = await TryExecuteAsync(dbCommand, ExecuteMode.ExecuteReader, i < DatabaseErrorsMaskManagerMaxRetries);
                dataReader = act.dataReader;

                if (act.result)
                    break;

                await Task.Delay(DatabaseErrorsMaskManagerQueryInterval);
            }

            return dataReader;
        }

        public async Task<T> ExecuteScalar<T>(IDbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            return await ExecuteScalar<T>(connection, null, commandText, parameters);
        }

        public async Task<T> ExecuteScalar<T>(IDbTransaction transaction, string commandText, params IDataParameter[] parameters)
        {
            return await ExecuteScalar<T>(transaction.Connection, transaction, commandText, parameters);
        }

        private async Task<T> ExecuteScalar<T>(IDbConnection connection, IDbTransaction transaction, string commandText, params IDataParameter[] parameters)
        {
            try
            {
                using DbCommand dbCommand = DbCommand(connection, commandText, parameters);
                if (transaction != null)
                {
                    DbTransaction dbTransaction = transaction as DbTransaction;
                    dbCommand.Transaction = dbTransaction;
                }

                return await ExecuteScalar<T>(dbCommand);

            }
            finally
            {
            }
        }

        private async Task<T> ExecuteScalar<T>(DbCommand dbCommand)
        {
            object res = null;

            for (int i = 1; i < DatabaseErrorsMaskManagerMaxRetries; i++)
            {
                (bool result, int rowsAffected, object singleResult, DbDataReader dataReader) = await TryExecuteAsync(dbCommand, ExecuteMode.ExecuteScalar, i < DatabaseErrorsMaskManagerMaxRetries);
                res = singleResult;

                if (result)
                    break;

                await Task.Delay(DatabaseErrorsMaskManagerQueryInterval);
            }

            return (T)res;
        }




    }
}
