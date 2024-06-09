using ServerCommonModule.Attributes;
using ServerCommonModule.Support;
using ServerCommonModule.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Model
{
    public abstract class ModelEntry : IModelEntry, IComparable<ModelEntry>
    {
        [FieldName("id"), FieldType(SqlDbType.UniqueIdentifier), FieldIsPrimaryKey, DisplayName("id")]
        public Guid Id { get; set; }

        [FieldName("name"), FieldType(SqlDbType.NVarChar), DisplayName("name")]
        public string Name { get; set; } = String.Empty;



        public ModelEntry()
        {
        }

        public virtual int CompareTo(ModelEntry other)
        {
            Debug.Assert(other != null);

            return other.Name.CompareTo(Name);
        }

        protected virtual Dictionary<Guid, object> GetDetailMapping()
        {
            Dictionary<Guid, object> mapping = new Dictionary<Guid, object>();

            return mapping;
        }

        public abstract IModelEntry Clone();





        public override string ToString()
        {
            return $"{Name} - {Id}";
        }


    }
}
