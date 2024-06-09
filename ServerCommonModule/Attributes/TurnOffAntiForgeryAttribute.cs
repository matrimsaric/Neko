using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Attributes
{
    /// <summary>
	/// Used when the system cannot legitimately expect the client to hold an anti forgery token (as it
	/// wont have been able to return one yet) AND for middleware that does not want to operate
	/// before the model is fully loaded - CacheValidationMiddleware for example.
	/// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TurnOffAntiForgeryAttribute : System.Attribute
    {
        public TurnOffAntiForgeryAttribute()
        {
        }
    }
}