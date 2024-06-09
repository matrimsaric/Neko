using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Support
{
    internal class Utility
    {
        #region Convert from SqlDbType to Type and the opposite
        public static SqlDbType ToSqlDbType(Type type)
        {
            if (type == typeof(string))
                return SqlDbType.NVarChar;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            return SqlDbType.NVarChar;
        }

        public static Type ToType(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    return typeof(long);

                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                case SqlDbType.Image:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.NText:
                case SqlDbType.Text:
                    return typeof(string);

                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:   // SQL2008
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);

                case SqlDbType.DateTimeOffset:  // SQL2008
                    return typeof(DateTimeOffset);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal);

                case SqlDbType.Float:
                    return typeof(double);
                case SqlDbType.Int:
                    return typeof(int);
                case SqlDbType.Real:
                    return typeof(float);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);
                case SqlDbType.SmallInt:
                    return typeof(short);

                case SqlDbType.Time:
                case SqlDbType.Timestamp:
                    return typeof(TimeSpan);

                case SqlDbType.TinyInt:
                    return typeof(byte);
                case SqlDbType.Xml:
                    return typeof(System.Data.SqlTypes.SqlXml);
                default:
                    return null;
            }
            // ignore special types: timestamp
            // ignore deprecated types: ntext, text
            // not supported : numeric, FILESTREAM, rowversion, sql_variant
        }

        #endregion Convert from SqlDbType to Type and the opposite
    }
}
