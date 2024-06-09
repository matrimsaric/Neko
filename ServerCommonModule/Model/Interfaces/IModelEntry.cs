using ServerCommonModule.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Model.Interfaces
{
    public interface IModelEntry
    {
        Guid Id { get; set; }
        string Name { get; set; }


    }
}