using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Control.Procedures
{
    public enum PROCEDURES
    {
        CLEAR_TEST_DATA = 0
    }
    internal class PermittedProcedures
    {
        internal string GetProcedureName(PROCEDURES procedure)
        {
            switch (procedure)
            {
                case PROCEDURES.CLEAR_TEST_DATA:
                    return "CALL test_tidy();";
            }
            return String.Empty;
        }
    }
}
