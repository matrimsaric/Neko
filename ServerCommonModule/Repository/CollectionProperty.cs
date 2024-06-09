using ServerCommonModule.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServerCommonModule.Repository
{
    public class CollectionProperty
    {
        public bool IsNullable { get; }
        public bool IsPrimaryKey { get; private set; }
        public SqlDbType FieldType { get; }
        public bool Uppercase { get; }
        public bool Identity { get; }
        public bool IsJson { get; }
        public string FieldName { get; private set; }
        public PropertyInfo Info { get; }
        public string DisplayName { get; private set; }

        public bool IsPublicNameProperty { get; set; }

        public CollectionProperty(PropertyInfo property, string fieldName)
        {
            FieldName = fieldName;

            Info = property;

            FieldTypeAttribute fieldTypeAttribute = (FieldTypeAttribute)property.GetCustomAttribute(typeof(FieldTypeAttribute));
            if (fieldTypeAttribute != null)
                FieldType = fieldTypeAttribute.FieldType;
            else
                FieldType = SqlDbType.NVarChar;

            FieldIsNullableAttribute isNullableAttribute = (FieldIsNullableAttribute)property.GetCustomAttribute(typeof(FieldIsNullableAttribute));
            if (isNullableAttribute != null)
                IsNullable = isNullableAttribute.IsNullable;

            FieldIsPrimaryKeyAttribute isPrimaryKeyAttribute = (FieldIsPrimaryKeyAttribute)property.GetCustomAttribute(typeof(FieldIsPrimaryKeyAttribute));
            if (isPrimaryKeyAttribute != null)
                IsPrimaryKey = isPrimaryKeyAttribute.IsPrimaryKey;



            FieldIdentityAttribute identityAttribute = (FieldIdentityAttribute)property.GetCustomAttribute(typeof(FieldIdentityAttribute));
            if (identityAttribute != null)
                Identity = identityAttribute.IsIdentity;

            DisplayNameAttribute displayNameAttribute = (DisplayNameAttribute)property.GetCustomAttribute(typeof(DisplayNameAttribute));
            if (displayNameAttribute != null)
                DisplayName = displayNameAttribute.DisplayName;
            else
                DisplayName = property.Name;

            IsPublicNameProperty = string.Equals(DisplayName, "Name", System.StringComparison.InvariantCultureIgnoreCase);

        }

        public void DisablePrimaryKey()
        {
            IsPrimaryKey = false;
        }
    }
}
