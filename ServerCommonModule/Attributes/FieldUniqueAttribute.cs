using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldUniqueAttribute : System.Attribute
    {
        public bool IsUnique { get; private set; }
        public FieldUniqueAttribute(bool isUnique)
        {
            IsUnique = isUnique;
        }
        public FieldUniqueAttribute()
           : this(false)
        {

        }
    }
}