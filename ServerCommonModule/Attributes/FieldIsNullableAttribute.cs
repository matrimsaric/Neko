using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldIsNullableAttribute : System.Attribute
    {
        public bool IsNullable { get; private set; }
        public FieldIsNullableAttribute(bool isNullable)
        {
            IsNullable = isNullable;
        }
        public FieldIsNullableAttribute()
           : this(true)
        {

        }
    }
}
