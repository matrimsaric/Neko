using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldTypeAttribute : System.Attribute
    {
        public SqlDbType FieldType { get; private set; }
        public FieldTypeAttribute(SqlDbType fieldType)
        {
            FieldType = fieldType;
        }
    }
}
