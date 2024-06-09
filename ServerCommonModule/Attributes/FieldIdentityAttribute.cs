using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldIdentityAttribute : System.Attribute
    {
        public bool IsIdentity { get; private set; }
        public FieldIdentityAttribute(bool isIdentity)
        {
            IsIdentity = isIdentity;
        }
        public FieldIdentityAttribute()
           : this(true)
        {

        }
    }
}