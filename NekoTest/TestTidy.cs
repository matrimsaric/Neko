using Microsoft.Data.SqlClient;
using ServerCommonModule.Control;
using ServerCommonModule.Control.Procedures;
using ServerCommonModule.Database;
using ServerCommonModule.Database.Interfaces;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;

namespace NekoTest;

[TestClass]
public class TestTidy
{
    [AssemblyCleanup()]
    public async static void AssemblyCleanup()
    {
        IEnvironmentalParameters envParms = new EnvironmentalParameters();
        envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";// TODO Note we need a much better single mechanism to do this for all classes
        envParms.DatabaseType = "PostgreSQL";
        IDbUtilityFactory  dbUtilityFactory = new PgUtilityFactory(envParms, null);

        DirectAccess directAccess = new DirectAccess(dbUtilityFactory, envParms);

        string sql = "exec test_tidy";

        List<Tuple<string,string, SqlDbType> > dbParameters = new List<Tuple<string, string, SqlDbType>>();

        Tuple<string, string, SqlDbType> targetParam = new Tuple<string, string, SqlDbType>("test_name", "TESTRANK", SqlDbType.NVarChar);
        dbParameters.Add(targetParam);

        await directAccess.ExecuteNonQueryStandAloneProcedure( PROCEDURES.CLEAR_TEST_DATA, null, dbParameters);
    }
}
