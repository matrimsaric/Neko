using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using ServerCommonModule.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Database
{
    public class PgUtility : DbUtility, IPgUtility
    {
        public Dictionary<SqlDbType, NpgsqlDbType> _mapSQLNpgType = [];
        public Dictionary<string, SqlDbType> _mapNpgsqlDbType = [];

        public PgUtility(IDbUtilityParameter dbUtilityParameter, IEnvironmentalParameters enviromentParameters)
            : base(dbUtilityParameter, enviromentParameters)
        {
            LoadMappingSqlType();
        }

        private void LoadMappingSqlType()
        {
            _mapSQLNpgType.Add(SqlDbType.DateTime, NpgsqlDbType.TimestampTz);
            _mapSQLNpgType.Add(SqlDbType.BigInt, NpgsqlDbType.Bigint);
            _mapSQLNpgType.Add(SqlDbType.Decimal, NpgsqlDbType.Numeric);
            _mapSQLNpgType.Add(SqlDbType.Int, NpgsqlDbType.Integer);
            _mapSQLNpgType.Add(SqlDbType.Money, NpgsqlDbType.Money);
            _mapSQLNpgType.Add(SqlDbType.NVarChar, NpgsqlDbType.Varchar);
            _mapSQLNpgType.Add(SqlDbType.SmallInt, NpgsqlDbType.Smallint);
            _mapSQLNpgType.Add(SqlDbType.TinyInt, NpgsqlDbType.Smallint);
            _mapSQLNpgType.Add(SqlDbType.UniqueIdentifier, NpgsqlDbType.Uuid);
            _mapSQLNpgType.Add(SqlDbType.VarBinary, NpgsqlDbType.Bytea);
            _mapSQLNpgType.Add(SqlDbType.Bit, NpgsqlDbType.Boolean);

            foreach (var rec in _mapSQLNpgType)
            {
                string pgkey = rec.Value.ToString().ToLowerInvariant();
                if (!_mapNpgsqlDbType.ContainsKey(pgkey))
                {
                    _mapNpgsqlDbType.Add(pgkey, rec.Key);
                }

            }
        }
        protected override async Task<IDbConnection> GetConnection(string connectionString)
        {
            Debug.Assert(string.IsNullOrEmpty(connectionString) == false);
            DbConnection connection = new NpgsqlConnection(connectionString);

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            return connection;
        }



        protected override bool ContainsAnErrorToCheck(DbException dbException)
        {
            if (dbException is PostgresException postgresException)
                if (DatabaseErrorsMaskManagerErrorCodes.Contains(postgresException.SqlState))
                    return true;

            return false;
        }

        public override IDataParameter CreateSqlParameter(string parameterName, SqlDbType parameterType, object parameterValue)
        {
            NpgsqlDbType npgsparam = GetNpgsqlDbType(parameterType);
            NpgsqlParameter lParameter = new()
            {
                ParameterName = parameterName,
                NpgsqlDbType = npgsparam,
                Value = parameterValue ?? DBNull.Value
            };
            return lParameter;
        }

        public NpgsqlDbType GetNpgsqlDbType(SqlDbType sqlDbType)
        {
            if (_mapSQLNpgType.TryGetValue(sqlDbType, out NpgsqlDbType npgsqlDBType) == true)
                return npgsqlDBType;

            return NpgsqlDbType.Varchar;
        }

        public SqlDbType GetSqlDbType(string pgDbType)
        {
            if (_mapNpgsqlDbType.TryGetValue(pgDbType, out SqlDbType sqlDbType) == true)
                return sqlDbType;

            if (pgDbType.StartsWith("timestamp")) return SqlDbType.DateTime;

            return SqlDbType.NVarChar;
        }

        protected override DbCommand DbCommand(IDbConnection connection, string commandText, params IDataParameter[] parameters)
        {
            DbCommand dbCommand = connection.CreateCommand() as DbCommand;
            dbCommand.CommandText = commandText;
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandTimeout = DatabaseCommandTimeoutInSeconds;
            if (parameters != null)
            {
                NpgsqlParameter[] sqlParameters = parameters.Cast<NpgsqlParameter>().ToArray();
                dbCommand.Parameters.AddRange(sqlParameters);
            }

            return dbCommand;
        }



    }
}
