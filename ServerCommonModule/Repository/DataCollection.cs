using ServerCommonModule.Attributes;
using ServerCommonModule.Repository.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommonModule.Repository
{
    public abstract class DataCollection<T> : ICollection<T>, IDataCollection
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        public const string EXT_PROP_LIST_VALUES = @"ListValues";

        public bool Sorted { get; private set; }

        public string Schema { get; set; } = string.Empty;

        public string TableName { get; set; }

        public string ArchiveTableName { get; set; }

        public string TableNameWithSchema
        {
            get
            {
                if (string.IsNullOrEmpty(Schema))
                    return TableName;
                else
                    return Schema + "." + TableName;
            }
        }

        public string ArchiveTableNameWithSchema
        {
            get
            {
                if (string.IsNullOrEmpty(Schema))
                    return ArchiveTableName;
                else
                    return Schema + "." + ArchiveTableName;
            }
        }


        public int Count
        {
            get
            {
                if (Sorted)
                    return _sortedItems.Count;
                else
                    return _items.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                if (Sorted)
                    return ((ICollection<T>)_sortedItems).IsReadOnly;
                else
                    return ((ICollection<T>)_items).IsReadOnly;
            }
        }

        private SortedSet<T> _sortedItems;

        private HashSet<T> _items;

        public DataCollection(bool sorted)
        {
            Sorted = sorted;

            if (Sorted)
                _sortedItems = new SortedSet<T>();
            else
                _items = new HashSet<T>();
        }

        public abstract T CreateItem();

        public void Add(T item)
        {
            if (Sorted)
                _sortedItems.Add(item);
            else
                _items.Add(item);
        }

        /// <summary>
        /// Add a new empty item to the collection. Additionally if it is a DD collection, set its CRUDStatus to Created and its Guid primary keys to new Guid
        /// </summary>
        public T AddEmptyItem()
        {
            T genericTObj = CreateItem();
            Add(genericTObj);

            List<PropertyInfo> tProperties = typeof(T).GetProperties(BINDING_FLAGS).ToList();
            foreach (PropertyInfo property in tProperties)
            {
                FieldNameAttribute fieldNameAttribute = (FieldNameAttribute)property.GetCustomAttribute(typeof(FieldNameAttribute));
                if (fieldNameAttribute == null)
                    continue;

                Type propertyType = property.PropertyType;
                if (propertyType != typeof(Guid))
                    continue;

                FieldIsPrimaryKeyAttribute isPrimaryKeyAttribute = (FieldIsPrimaryKeyAttribute)property.GetCustomAttribute(typeof(FieldIsPrimaryKeyAttribute));
                if (isPrimaryKeyAttribute == null)
                    continue;

                if (isPrimaryKeyAttribute.IsPrimaryKey)
                    property.SetValue(genericTObj, Guid.NewGuid());
            }

            return genericTObj;
        }

        public void AddRange(List<T> items)
        {
            if (Sorted)
                _sortedItems.UnionWith(items);
            else
                _items.UnionWith(items);
        }

        public void RemoveWhere(Predicate<T> clause)
        {
            if (Sorted)
                _sortedItems.RemoveWhere(clause);
            else
                _items.RemoveWhere(clause);
        }

        public void CopyToByClause(DataCollection<T> destinationCollection, Func<T, bool> clause)
        {
            IEnumerable whereResults = null;

            if (Sorted)
                whereResults = _sortedItems.Where(clause);
            else
                whereResults = _items.Where(clause);

            foreach (T item in whereResults)
                destinationCollection.Add(item);
        }

        public void Clear()
        {
            if (Sorted)
                _sortedItems.Clear();
            else
                _items.Clear();
        }

        public bool Contains(T item)
        {
            if (Sorted)
                return _sortedItems.Contains(item);
            else
                return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (Sorted)
                _sortedItems.CopyTo(array, arrayIndex);
            else
                _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (Sorted)
                return _sortedItems.Remove(item);
            else
                return _items.Remove(item);
        }

        public void Remove(List<T> items)
        {
            if (Sorted)
                foreach (T item in items)
                    _sortedItems.Remove(item);
            else
                foreach (T item in items)
                    _items.Remove(item);
        }

        public void Remove(DataCollection<T> items)
        {
            if (Sorted)
                foreach (T item in items)
                    _sortedItems.Remove(item);
            else
                foreach (T item in items)
                    _items.Remove(item);
        }



        public IEnumerator<T> GetEnumerator()
        {
            if (Sorted)
                return _sortedItems.GetEnumerator();
            else
                return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            List<PropertyInfo> tProperties = typeof(T).GetProperties(BINDING_FLAGS).ToList();
            DataColumn newColumn = null;
            HashSet<DataColumn> primaryKeys = new HashSet<DataColumn>();
            foreach (PropertyInfo property in tProperties)
            {
                Type propertyType = property.PropertyType;
                if (propertyType.IsEnum)
                    propertyType = typeof(string);

                if (propertyType.Name.Contains("Nullable"))
                    newColumn = new DataColumn(property.Name, Nullable.GetUnderlyingType(propertyType));
                else
                    newColumn = new DataColumn(property.Name, propertyType);

                DisplayNameAttribute displayNameAttribute = (DisplayNameAttribute)property.GetCustomAttribute(typeof(DisplayNameAttribute));
                if (displayNameAttribute != null)
                    newColumn.Caption = displayNameAttribute.DisplayName;

                if (property.PropertyType.IsEnum)
                {
                    Dictionary<object, string> listValues = Enum.GetNames(property.PropertyType).ToDictionary(x => x as object, x => x);
                    newColumn.ExtendedProperties.Add(EXT_PROP_LIST_VALUES, listValues);
                }

                FieldIsPrimaryKeyAttribute isPrimaryKeyAttribute = (FieldIsPrimaryKeyAttribute)property.GetCustomAttribute(typeof(FieldIsPrimaryKeyAttribute));
                if (isPrimaryKeyAttribute != null)
                {
                    primaryKeys.Add(newColumn);
                    newColumn.ReadOnly = true;
                }

                FieldNameAttribute fieldNameAttribute = (FieldNameAttribute)property.GetCustomAttribute(typeof(FieldNameAttribute));
                if (fieldNameAttribute == null)
                    newColumn.ReadOnly = true;

                dataTable.Columns.Add(newColumn);
            }
            dataTable.PrimaryKey = primaryKeys.ToArray();

            if (this.Count == 0)
                return dataTable;

            foreach (T item in this)
            {
                DataRow dataRow = dataTable.NewRow();
                foreach (PropertyInfo propertyInfo in tProperties)
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(item, null) == null ? DBNull.Value : propertyInfo.GetValue(item, null);

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public void ToCollection(DataTable dataTable)
        {
            this.Clear();

            List<PropertyInfo> tProperties = typeof(T).GetProperties(BINDING_FLAGS).ToList();

            foreach (DataRow row in dataTable.Rows)
            {
                T genericTObj = CreateItem();
                Debug.Assert(genericTObj != null);

                int i = 0;
                foreach (PropertyInfo property in tProperties)
                {
                    if (property.PropertyType.IsEnum)
                        property.SetValue(genericTObj, Enum.Parse(property.PropertyType, row[i].ToString()));
                    else
                    {
                        if (Convert.IsDBNull(row[i]))
                            property.SetValue(genericTObj, null);
                        else
                            property.SetValue(genericTObj, row[i]);
                    }
                    i++;
                }

                this.Add(genericTObj);
            }
        }

        private Dictionary<PropertyInfo, PropertyInfo> GetCommonKeysByParentPropertyInfo(PropertyInfoGroups properties, PropertyInfoGroups masterProperties)
        {
            Dictionary<PropertyInfo, PropertyInfo> commonKeys = new Dictionary<PropertyInfo, PropertyInfo>();

            foreach (PropertyInfo key in properties.PrimaryKeys)
            {
                PropertyInfo masterKey = masterProperties.PrimaryKeys.FirstOrDefault(x => x.Name == key.Name);

                if (masterKey != null)
                    commonKeys.Add(key, masterKey);
            }

            Debug.Assert(commonKeys.Count > 0);
            return commonKeys;
        }

        private Dictionary<PropertyInfo, PropertyInfo> GetParentKeysToPopulatePropertyInfo(PropertyInfoGroups properties, PropertyInfoGroups masterProperties)
        {
            Dictionary<PropertyInfo, PropertyInfo> commonParentKeys = new Dictionary<PropertyInfo, PropertyInfo>();

            foreach (PropertyInfo parentKey in properties.ParentKeys)
            {
                PropertyInfo masterKey = masterProperties.PrimaryKeys.FirstOrDefault(x => x.Name == parentKey.Name);

                if (masterKey != null)
                    commonParentKeys.Add(parentKey, masterKey);
                else
                {
                    masterKey = masterProperties.OtherProperties.FirstOrDefault(x => x.Name == parentKey.Name);
                    if (masterKey != null)
                        commonParentKeys.Add(parentKey, masterKey);
                    else
                    {
                        masterKey = masterProperties.ParentKeys.FirstOrDefault(x => x.Name == parentKey.Name);
                        if (masterKey != null)
                            commonParentKeys.Add(parentKey, masterKey);
                    }
                }
            }

            Debug.Assert(commonParentKeys.Count > 0);
            return commonParentKeys;
        }

        private TParent FindParentByCommonKey<TParent>(Dictionary<PropertyInfo, PropertyInfo> commonKeys, T item, DataCollection<TParent> parents)
        {
            foreach (TParent parent in parents)
            {
                bool found = true;
                foreach (KeyValuePair<PropertyInfo, PropertyInfo> kvp in commonKeys)
                    if (object.Equals(kvp.Key.GetValue(item), kvp.Value.GetValue(parent)) == false)
                    {
                        found = false;
                        break;
                    }

                if (found)
                    return parent;
            }

            return default(TParent);
        }









        private PropertyInfo GetPropertyByFieldName(PropertyInfoGroups propertyGroups, string fieldName)
        {
            PropertyInfo masterExternalKeyFieldValueProperty = GetPropertyByFieldName(propertyGroups.PrimaryKeys, fieldName);
            if (masterExternalKeyFieldValueProperty == null)
                masterExternalKeyFieldValueProperty = GetPropertyByFieldName(propertyGroups.OtherProperties, fieldName);

            return masterExternalKeyFieldValueProperty;
        }

        private PropertyInfo GetPropertyByFieldName(List<PropertyInfo> properties, string fieldName)
        {
            foreach (PropertyInfo property in properties)
            {
                FieldNameAttribute fieldNameAttribute = (FieldNameAttribute)property.GetCustomAttribute(typeof(FieldNameAttribute));
                if (fieldNameAttribute == null)
                    continue;

                if (fieldNameAttribute.FieldName.Equals(fieldName))
                    return property;
            }

            return null;
        }







        protected T FindByKey(PropertyInfoGroups groups, T dd)
        {
            foreach (T dm in this)
            {
                bool found = true;
                foreach (PropertyInfo key in groups.PrimaryKeys)
                    if (object.Equals(key.GetValue(dm), key.GetValue(dd)) == false)
                    {
                        found = false;
                        break;
                    }

                if (found)
                    return dm;
            }

            return default(T);
        }

        private void Update(PropertyInfoGroups groups, T dm, T dd)
        {
            foreach (PropertyInfo other in groups.OtherProperties)
                other.SetValue(dm, other.GetValue(dd));
        }

        protected bool CompareProperties(PropertyInfoGroups groups, T dm, T dd)
        {
            foreach (PropertyInfo other in groups.OtherProperties)
                if (object.Equals(other.GetValue(dm), other.GetValue(dd)) == false)
                    return false;

            return true;
        }



        private HashSet<CollectionProperty> GetCollectionProperties()
        {
            HashSet<CollectionProperty> collectionProperties = new HashSet<CollectionProperty>();

            foreach (PropertyInfo property in typeof(T).GetProperties(DataCollection<T>.BINDING_FLAGS))
            {
                if (property.GetCustomAttributes().Count() > 0)
                {
                    FieldNameAttribute fieldNameAttribute = (FieldNameAttribute)property.GetCustomAttribute(typeof(FieldNameAttribute));
                    string fieldName = string.Empty;
                    if (fieldNameAttribute != null)
                        fieldName = fieldNameAttribute.FieldName;


                    if (string.IsNullOrEmpty(fieldName) == false)
                        collectionProperties.Add(new CollectionProperty(property, fieldName));
                }
            }


            return collectionProperties;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (T item in this)
            {
                builder.AppendLine(item.ToString());
            }

            return builder.ToString();
        }
    }

    public class PropertyInfoGroups
    {
        public PropertyInfo ExternalId { get; set; }

        public List<PropertyInfo> PrimaryKeys { get; private set; } = new List<PropertyInfo>();

        public List<PropertyInfo> OtherProperties { get; private set; } = new List<PropertyInfo>();

        public List<PropertyInfo> ParentKeys { get; private set; } = new List<PropertyInfo>();

        public List<PropertyInfo> NonKeyMandatoryProperties { get; set; } = new List<PropertyInfo>();

        public PropertyInfo ExternalKey { get; set; }
        public PropertyInfo ExternalKeyValue { get; set; }



    }
}