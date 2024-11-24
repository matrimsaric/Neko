using ServerCommonModule.Control.Procedures;
using ServerCommonModule.Database;
using ServerCommonModule.Database.Interfaces;
using ServerCommonModule.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Control
{
    public class DirectAccess
    {
        private readonly IEnvironmentalParameters _environmentParameters = null;
        private readonly IDbUtility _dbUtility;
        private readonly PermittedProcedures permittedProcedures = new PermittedProcedures();

        public DirectAccess(IDbUtilityFactory dbUtilityFactory, IEnvironmentalParameters environmentParameters)
        {
            _dbUtility = dbUtilityFactory.Get();
            _environmentParameters = environmentParameters;

        }

        public async Task ExecuteNonQueryStandAloneProcedure(PROCEDURES procedureRequested, IDbTransaction transaction, bool forceParams, List<Tuple<string,string, SqlDbType>> partialQueryParameters = null)
        {
            string procedureToRun = permittedProcedures.GetProcedureName(procedureRequested);

            if (string.IsNullOrEmpty(procedureToRun))
            {
                throw new Exception("Invalid Procedure");
            }
            List<IDataParameter> allParameters = GetDataParameters(partialQueryParameters);

            using (IDbConnection dbConnection = await _dbUtility.GetConnection())
            {
                if (forceParams)
                {
                    // only force params for known internal procs. Else must use DbParameter objects
                    string paramList = String.Empty;

                    foreach(var paramer in allParameters)
                    {
                        if (!String.IsNullOrEmpty(paramList))
                        {
                            paramList = $"'{paramList}','{paramer.Value.ToString()}'";
                        }
                        else
                        {
                            paramList = $"'{paramer.Value.ToString()}'";
                        }
                        
                    }
                    procedureToRun = procedureToRun.Replace("(", "( " + paramList);
                    await _dbUtility.ExecuteNonQuery(dbConnection, procedureToRun );
                }
                else
                {
                    if (transaction != null)
                    {
                        await _dbUtility.ExecuteNonQuery(transaction, procedureToRun, allParameters.ToArray());
                    }
                    else
                    {
                        await _dbUtility.ExecuteNonQuery(dbConnection, procedureToRun, allParameters.ToArray());
                    }
                }
                
            }

            return;

           
        }

        private List<IDataParameter> GetDataParameters(List<Tuple<string,string, SqlDbType>> requestedParams)
        {
            List<IDataParameter> allParameters = new List<IDataParameter>();

            foreach (var parameter in requestedParams)
            {
                allParameters.Add(_dbUtility.CreateSqlParameter(parameter.Item1 as string, parameter.Item3, parameter.Item2));
            }
            return allParameters;
        }




    }
}
