using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldNameAttribute : System.Attribute
    {
        public string FieldName { get; private set; }
        public FieldNameAttribute(string fieldName)
        {
            Debug.Assert(string.IsNullOrEmpty(fieldName) == false);
            FieldName = fieldName;
        }
    }
}
