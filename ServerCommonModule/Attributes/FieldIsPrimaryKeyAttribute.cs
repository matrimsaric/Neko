using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldIsPrimaryKeyAttribute : System.Attribute
    {
        public bool IsPrimaryKey { get; private set; }
        public FieldIsPrimaryKeyAttribute(bool isPrimaryKey)
        {
            IsPrimaryKey = isPrimaryKey;
        }
        public FieldIsPrimaryKeyAttribute()
           : this(true)
        {

        }
    }
}
