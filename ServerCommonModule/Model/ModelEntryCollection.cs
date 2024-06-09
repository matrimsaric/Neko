using ServerCommonModule.Repository;
using ServerCommonModule.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Model
{
    public class ModelEntryCollection<T> : DataCollection<T> where T : IModelEntry, new()
    {
        public ModelEntryCollection(bool sorted)
            : base(sorted)
        {
        }




        public override T CreateItem()
        {
            return new T();
        }

        public HashSet<string> GetExistingNames()
        {
            HashSet<string> existingNames = new HashSet<string>();
            foreach (IModelEntry modelEntry in this)
            {
                existingNames.Add(modelEntry.Name);
            }

            return existingNames;
        }


        public virtual string GetBaseName()
        {
            return string.Empty;
        }

        public virtual string GetBaseExternalId()
        {
            return string.Empty;
        }

        public T FindById(Guid id)
        {
            return this.FirstOrDefault(x => x.Id.Equals(id));
        }


        public List<T> FindByName(string name)
        {
            return this.Where(x => x.Name.Equals(name)).ToList();
        }
    }
}
