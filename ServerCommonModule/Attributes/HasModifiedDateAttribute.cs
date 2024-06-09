using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HasModifiedDateAttribute : System.Attribute
    {
        public bool HasModifiedDate { get; private set; }

        public HasModifiedDateAttribute(bool hasModifiedDate)
        {
            HasModifiedDate = hasModifiedDate;
        }

        public HasModifiedDateAttribute() : this(true) { }
    }
}
