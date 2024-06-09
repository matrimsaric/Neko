using ServerCommonModule.Attributes;
using ServerCommonModule.Repository.Interfaces;
using ServerCommonModule.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServerCommonModule.Model;
using ServerCommonModule.Support;

namespace ServerCommonModule.Repository
{
    public partial class RepositoryManager<T> : IRepositoryManager<T>
    {
        protected DataCollection<T> _collection = null;
        private bool _hasModifiedDate = true;
        private readonly Dictionary<string, CollectionProperty> _collectionProperties = new Dictionary<string, CollectionProperty>();
        private const string MODIFIED_FIELD = "modified_date";

        private readonly IDbUtility _dbUtility;
        private readonly IEnvironmentalParameters _environmentalParameters;

        private enum FieldsGetter
        {
            PrimaryKeys,
            NotPrimaryKeys,
            /// <summary> Similar to "NotPrimaryKeys" but without identity fields. Because if an UPDATE is created using NotPrimaryKeys it will fail on identity fields. </summary>
            NotPrimaryKeysUpdeatable,
            All,
            /// <summary> Similar to "All" but without identity fields. Because if an INSERT is created using all it will fail on identity fields. </summary>
            AllInsertable
        }

        public RepositoryManager(IDbUtilityFactory dbUtilityFactory, IEnvironmentalParameters environmentParameters)
        {
            _dbUtility = dbUtilityFactory.Get();
            _environmentalParameters = environmentParameters;

        }



        public void SetCollection(DataCollection<T> collection)
        {
            _collection = collection;
            GetTableProperties();
            GetCollectionProperties();
        }



        #region Private methods

        private async Task Load(IDbConnection connection, IDbTransaction transaction, string query, List<string> whereConditions, List<IDataParameter> whereParameters)
        {
            if (whereConditions.Count > 0)
            {
                query += "\nWHERE\n\t" + string.Join("\tAND ", whereConditions);

                if (transaction != null)
                    using (IDataReader dataReader = await _dbUtility.ExecuteReader(transaction, query, whereParameters.ToArray()))
                        GetItems(dataReader);
                else
                    using (IDataReader dataReader = await _dbUtility.ExecuteReader(connection, query, whereParameters.ToArray()))
                        GetItems(dataReader);
            }
            else
            {
                if (transaction != null)
                    using (IDataReader dataReader = await _dbUtility.ExecuteReader(transaction, query))
                        GetItems(dataReader);
                else
                    using (IDataReader dataReader = await _dbUtility.ExecuteReader(connection, query))
                        GetItems(dataReader);
            }


        }



        /// <summary>
        /// Adds where condition on additional properties to filter the loading collection
        /// </summary>
        private void GetAdditionalWhereCondition(List<string> whereConditions, List<IDataParameter> whereParameters, string tablePrefix = "")
        {

            IEnumerable<KeyValuePair<string, CollectionProperty>> fields = _collectionProperties.Where(x => x.Value.IsPrimaryKey);
            List<string> keysWhereCondition = new List<string>();

            int parameterIndex = 1;
            T defaultItem = _collection.CreateItem();
            bool whereExists = false;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < keysWhereCondition.Count; i++)
            {
                whereExists = true;
                if (i == 0 && keysWhereCondition.Count > 1)
                    builder.Append("(");

                builder.Append(keysWhereCondition[i]);

                if (i < keysWhereCondition.Count - 1)
                    builder.Append(" OR ");
                else if (i == keysWhereCondition.Count - 1 && keysWhereCondition.Count > 1)
                    builder.Append(")");
            }
            if (whereExists == true) whereConditions.Add(builder.ToString());
        }





        private IDataParameter CreateSqlParameter(string parameterName, SqlDbType dbType, object value)
        {
            if (dbType == SqlDbType.UniqueIdentifier)
            {
                Guid guidValue = value is Guid ? (Guid)value : Guid.Parse((string)value);
                return _dbUtility.CreateSqlParameter(parameterName, dbType, guidValue);
            }
            else
            {
                return _dbUtility.CreateSqlParameter(parameterName, dbType, value);
            }
        }

        private void GetItems(IDataReader dataReader)
        {
            while (dataReader.Read())
            {
                T genericTObj = _collection.CreateItem();
                Debug.Assert(genericTObj != null);

                int i = 0;
                foreach (CollectionProperty field in _collectionProperties.Values)
                {
                    if (dataReader.IsDBNull(i))
                    {
                        i++;
                        continue;
                    }

                    PropertyInfo propertyInfo = field.Info;
                    bool uppercase = field.Uppercase;

                    if (propertyInfo.PropertyType == typeof(Version))
                    {
                        Version version;
                        if (Version.TryParse(dataReader.GetString(i), out version))
                            propertyInfo.SetValue(genericTObj, version);
                    }
                    else if (propertyInfo.PropertyType.IsEnum)
                    {
                        Type fieldType = dataReader.GetFieldType(i);
                        if (fieldType == typeof(string))
                        {
                            propertyInfo.SetValue(genericTObj, Enum.ToObject(propertyInfo.PropertyType, Enum.Parse(propertyInfo.PropertyType, dataReader.GetString(i), true)), null);
                        }
                        else if (fieldType == typeof(int))
                            propertyInfo.SetValue(genericTObj, Enum.ToObject(propertyInfo.PropertyType, dataReader.GetInt32(i)), null);
                        else
                            throw new ApplicationException();
                    }
                    else
                    {
                        switch (field.FieldType)
                        {
                            case SqlDbType.TinyInt:
                                propertyInfo.SetValue(genericTObj, dataReader.GetByte(i));
                                break;
                            case SqlDbType.SmallInt:
                                propertyInfo.SetValue(genericTObj, dataReader.GetInt16(i));
                                break;
                            case SqlDbType.Int:
                                propertyInfo.SetValue(genericTObj, dataReader.GetInt32(i));
                                break;
                            case SqlDbType.BigInt:
                                propertyInfo.SetValue(genericTObj, dataReader.GetInt64(i));
                                break;
                            case SqlDbType.Decimal:
                                propertyInfo.SetValue(genericTObj, dataReader.GetDecimal(i));
                                break;
                            case SqlDbType.VarChar:
                            case SqlDbType.NVarChar:
                                if (uppercase)
                                    propertyInfo.SetValue(genericTObj, dataReader.GetString(i).ToUpper());
                                else
                                    propertyInfo.SetValue(genericTObj, dataReader.GetString(i));
                                break;
                            case SqlDbType.Bit:
                                propertyInfo.SetValue(genericTObj, dataReader.GetBoolean(i));
                                break;
                            case SqlDbType.DateTime:
                            case SqlDbType.Date:
                                propertyInfo.SetValue(genericTObj, dataReader.GetDateTime(i));
                                break;
                            case SqlDbType.UniqueIdentifier:
                                propertyInfo.SetValue(genericTObj, dataReader.GetGuid(i));
                                break;
                            case SqlDbType.VarBinary:
                                long len = dataReader.GetBytes(i, 0, null, 0, 0);
                                Byte[] buffer = new Byte[len];
                                dataReader.GetBytes(i, 0, buffer, 0, (int)len);
                                propertyInfo.SetValue(genericTObj, buffer);
                                break;
                        }
                    }
                    i++;
                }

                _collection.Add(genericTObj);
            }
        }

        private string GetNotMatchedSearchConditionSingleItem(string query, bool isDDTable, bool hasFilterParameters, List<IDataParameter> mergeParameters)
        {
            if (string.IsNullOrEmpty(query) == false)
                query += " AND ";

            if (isDDTable)
            {
                query += "feature_key = @FeatureKey";
            }

            if (string.IsNullOrEmpty(query) == false && hasFilterParameters)
                query += " AND ";

            if (hasFilterParameters)
            {
                List<String> whereConditions = new List<string>();
                List<IDataParameter> whereParameters = new List<IDataParameter>();
                GetAdditionalWhereCondition(whereConditions, whereParameters);
                mergeParameters.AddRange(whereParameters);
                query += string.Join(" AND ", whereConditions);
            }

            return query;
        }

        private string GetNotMatchedSearchCondition(string partialQuery, bool isDDTable, bool hasFilterParameters, List<IDataParameter> mergeParameters)
        {
            string query = string.IsNullOrEmpty(partialQuery) ? string.Empty : "AND" + partialQuery;

            if (hasFilterParameters)
            {
                List<string> whereConditions = new List<string>();
                List<IDataParameter> whereParameters = new List<IDataParameter>();
                GetAdditionalWhereCondition(whereConditions, whereParameters, "target.");
                mergeParameters.AddRange(whereParameters);
                query += " AND " + string.Join(" AND ", whereConditions);
            }

            return query;
        }

        private async Task Save(IDbConnection connection, IDbTransaction transaction, string partialQuery = "", List<IDataParameter> partialQueryParameters = null)
        {


            GetStatements(partialQuery, partialQueryParameters, out string tempTablePrefix, out List<IDataParameter> mergeParameters, out DataTable tableToSave, out List<string> allFieldList, out string temporaryTableCreationQuery, out string preMergeDelete, out StringBuilder mergeStatement);
            string destinationTable = tempTablePrefix + _collection.TableName;

            if (transaction != null)
            {
                await _dbUtility.ExecuteNonQuery(transaction, temporaryTableCreationQuery);

                if (string.IsNullOrEmpty(preMergeDelete) == false)
                    await _dbUtility.ExecuteNonQuery(transaction, preMergeDelete, mergeParameters.ToArray());

                await _dbUtility.ExecuteNonQuery(transaction, mergeStatement.ToString(), mergeParameters.ToArray());
            }
            else
            {
                await _dbUtility.ExecuteNonQuery(connection, temporaryTableCreationQuery);

                allFieldList = GetFields(FieldsGetter.All, false);

                if (string.IsNullOrEmpty(preMergeDelete) == false)
                    await _dbUtility.ExecuteNonQuery(connection, preMergeDelete, mergeParameters.ToArray());

                await _dbUtility.ExecuteNonQuery(connection, mergeStatement.ToString(), mergeParameters.ToArray());
            }
        }

        private void GetStatements(string partialQuery, List<IDataParameter> partialQueryParameters, out string tempTablePrefix, out List<IDataParameter> mergeParameters, out DataTable tableToSave, out List<string> allFieldList, out string temporaryTableCreationQuery, out string preMergeDelete, out StringBuilder mergeStatement)
        {
            mergeParameters = new List<IDataParameter>();
            tableToSave = GetCollectionDataTable();
            allFieldList = GetFields(FieldsGetter.AllInsertable, true);
            List<string> primaryKeyList = GetFields(FieldsGetter.PrimaryKeys, true);
            List<string> updateFieldList = GetFields(FieldsGetter.NotPrimaryKeysUpdeatable, true);

            List<string> temporaryTableFieldList = GetFields(FieldsGetter.All, true);

            preMergeDelete = string.Empty;
            mergeStatement = new StringBuilder();

            switch (_environmentalParameters.Database)
            {
                case ConnectionType.POSTGRESS:
                    {
                        tempTablePrefix = "tt_";

                        temporaryTableCreationQuery = $@"
create temporary table if not exists  {tempTablePrefix}{_collection.TableName} as
SELECT {string.Join(", ", temporaryTableFieldList)} FROM  {_collection.TableNameWithSchema} WHERE 1=0;
";

                        string checkOnPrimaryKeys = string.Join(" AND ", primaryKeyList.Select(x => "target." + x + " = source." + x));
                        string updateFields = string.Join(", ", updateFieldList.Select(x => x + " = source." + x));

                        string sourceTableName = tempTablePrefix + _collection.TableName;

                        string getNotMatchedSearchCondition = GetNotMatchedSearchCondition(partialQuery, false, false, mergeParameters);
                        if (string.IsNullOrEmpty(getNotMatchedSearchCondition) == false)
                        {
                            bool startsWithAnd = getNotMatchedSearchCondition.TrimStart().StartsWith("AND");
                            if (startsWithAnd == true) getNotMatchedSearchCondition = getNotMatchedSearchCondition + Environment.NewLine;
                            else getNotMatchedSearchCondition = " AND " + getNotMatchedSearchCondition + Environment.NewLine;
                        }

                        preMergeDelete = $"DELETE FROM {_collection.TableNameWithSchema} AS target" + Environment.NewLine +
                            $"WHERE NOT EXISTS(" + Environment.NewLine +
                            $"	SELECT NULL" + Environment.NewLine +
                            $"  FROM {sourceTableName} source " + Environment.NewLine +
                            $"  WHERE ({checkOnPrimaryKeys})" + Environment.NewLine +
                            getNotMatchedSearchCondition +
                            $")";

                        mergeStatement.Append($"MERGE INTO {_collection.TableNameWithSchema} AS target" + Environment.NewLine +
                            $"USING {tempTablePrefix}{_collection.TableName} AS source" + Environment.NewLine +
                            $"  ON ({checkOnPrimaryKeys})");

                        if (string.IsNullOrEmpty(updateFields) == false)
                        {
                            mergeStatement.AppendLine($"WHEN MATCHED THEN" + Environment.NewLine +
                            $"  UPDATE SET {updateFields}");
                        }

                        mergeStatement.AppendLine($"WHEN NOT MATCHED THEN" + Environment.NewLine +
                            $"  INSERT" + Environment.NewLine +
                            $"      ({string.Join(", ", allFieldList)})" + Environment.NewLine +
                            $"  VALUES" + Environment.NewLine +
                            $"      (source.{string.Join(", source.", allFieldList)})" + Environment.NewLine +
                            $";");

                        break;
                    }
                case ConnectionType.MS_SQL:
                default:
                    {
                        tempTablePrefix = (string.IsNullOrEmpty(_collection.Schema) ? "#" : _collection.Schema + ".#");

                        temporaryTableCreationQuery = $"IF (OBJECT_ID('tempdb..{tempTablePrefix}{_collection.TableName}') IS NULL) BEGIN SELECT {string.Join(", ", temporaryTableFieldList)} INTO {tempTablePrefix}{_collection.TableName} FROM {_collection.TableNameWithSchema} WHERE 1=0 END";
                        string checkOnPrimaryKeys = string.Join(" AND ", primaryKeyList.Select(x => "target." + x + " = source." + x));
                        string updateFields = string.Join(", ", updateFieldList.Select(x => "target." + x + " = source." + x));

                        string sourceTableName = tempTablePrefix + _collection.TableName;

                        mergeStatement.Append($"MERGE {_collection.TableNameWithSchema} AS target" + Environment.NewLine +
                            $"USING {tempTablePrefix}{_collection.TableName} AS source" + Environment.NewLine +
                            $"  ON({checkOnPrimaryKeys})");

                        if (string.IsNullOrEmpty(updateFields) == false)
                        {
                            mergeStatement.AppendLine($"WHEN MATCHED THEN" + Environment.NewLine +
                            $"  UPDATE SET {updateFields}");
                        }

                        mergeStatement.AppendLine($"WHEN NOT MATCHED BY target THEN" + Environment.NewLine +
                            $"  INSERT" + Environment.NewLine +
                            $"      ({string.Join(", ", allFieldList)})" + Environment.NewLine +
                            $"  VALUES" + Environment.NewLine +
                            $"      (source.{string.Join(", source.", allFieldList)})" + Environment.NewLine +
                            $"WHEN NOT MATCHED BY source" + Environment.NewLine +
                            GetNotMatchedSearchCondition(partialQuery, false, false, mergeParameters) +
                            $"  THEN DELETE" + Environment.NewLine +
                            $";");

                        break;
                    }
            }

            if (partialQueryParameters != null)
                mergeParameters.AddRange(partialQueryParameters);
        }

        private async Task ExecuteSingleItemAction(IDbConnection connection, IDbTransaction transaction, T item, bool insert, bool update, bool delete, string partialQuery = "", List<IDataParameter> partialQueryParameters = null)
        {

            Dictionary<string, IDataParameter> itemParameters = GetSingleItemAsParameters(item);
            List<IDataParameter> allParameters = new List<IDataParameter>();

            string statement = ExecuteSingleActionPrepareQueries(insert, update, delete, partialQuery, partialQueryParameters, itemParameters, allParameters);

            if (transaction != null)
            {
                await _dbUtility.ExecuteNonQuery(transaction, statement, allParameters.ToArray());
            }
            else
            {
                await _dbUtility.ExecuteNonQuery(connection, statement, allParameters.ToArray());
            }
        }

        private string ExecuteSingleActionPrepareQueries(bool insert, bool update, bool delete, string partialQuery, List<IDataParameter> partialQueryParameters, Dictionary<string, IDataParameter> itemParameters, List<IDataParameter> allParameters)
        {
            List<string> allFieldList = GetFields(FieldsGetter.AllInsertable, true);
            List<string> primaryKeyList = GetFields(FieldsGetter.PrimaryKeys, true);
            List<string> updateFieldList = GetFields(FieldsGetter.NotPrimaryKeysUpdeatable, true);


            string whereCondition = string.Join(" AND ", primaryKeyList.Select(x => x + " = " + itemParameters[x].ParameterName));
            string tempPartial = GetNotMatchedSearchConditionSingleItem(partialQuery, false, false, allParameters);
            if (string.IsNullOrEmpty(tempPartial) == false)
            {
                if (string.IsNullOrEmpty(whereCondition) == false)
                    whereCondition += " AND " + tempPartial;
                else
                    whereCondition = tempPartial;
            }
            AddParametersWithReplace(allParameters, partialQueryParameters);
            AddParametersWithReplace(allParameters, itemParameters.Values);

            string statement = string.Empty;
            if (insert)
            {

                statement = @"
    INSERT INTO " + _collection.TableNameWithSchema + @"
    (" + string.Join(", ", allFieldList) + @")
    VALUES
    (" + string.Join(", ", allFieldList.Select(x => itemParameters[x].ParameterName)) + @")
    ";
            }
            else if (update)
            {
                statement = @"
    UPDATE " + _collection.TableNameWithSchema + @" SET
    " + string.Join(", ", updateFieldList.Select(x => x + " = " + itemParameters[x].ParameterName)) + @"
    WHERE
    " + whereCondition + @"
    ";
            }
            else if (delete)
            {
                statement = @"
    DELETE FROM " + _collection.TableNameWithSchema + @" 
    WHERE
    " + whereCondition + @"
    ";
            }

            return statement;
        }

        private static void AddParametersWithReplace(List<IDataParameter> masterParameters, IEnumerable<IDataParameter> additionalParameters)
        {
            if (additionalParameters != null)
            {
                foreach (DbParameter param in additionalParameters)
                {
                    IDataParameter duplicateParameter = masterParameters.FirstOrDefault(x => string.Equals(param.ParameterName, x.ParameterName, StringComparison.InvariantCultureIgnoreCase));
                    if (duplicateParameter != null)
                        masterParameters.Remove(duplicateParameter);
                    masterParameters.Add(param);
                }
            }
        }

        private void GetTableProperties()
        {
            foreach (var custAtt in typeof(T).GetCustomAttributes())
            {
                if (custAtt is TableAttribute)
                {
                    TableAttribute nameFld = custAtt as TableAttribute;


                    _collection.TableName = nameFld.Name;
                }
            }
        }



        private void GetCollectionProperties()
        {
            HasModifiedDateAttribute hasModifiedDateAttribute = (HasModifiedDateAttribute)typeof(T).GetCustomAttribute(typeof(HasModifiedDateAttribute));
            if (hasModifiedDateAttribute != null)
                _hasModifiedDate = hasModifiedDateAttribute.HasModifiedDate;

            foreach (PropertyInfo property in typeof(T).GetProperties(DataCollection<T>.BINDING_FLAGS))
            {
                if (property.GetCustomAttributes().Count() > 0)
                {
                    FieldNameAttribute fieldNameAttribute = (FieldNameAttribute)property.GetCustomAttribute(typeof(FieldNameAttribute));
                    string fieldName = string.Empty;
                    if (fieldNameAttribute != null)
                        fieldName = fieldNameAttribute.FieldName;



                    if (string.IsNullOrEmpty(fieldName) == false)
                        _collectionProperties.Add(fieldName, new CollectionProperty(property, fieldName));
                }
            }


        }

        private string GenerateSelectQuery(bool useReadUncommitted)
        {
            string sql = $"SELECT \n\t{string.Join(",\n\t", GetFields(FieldsGetter.All, true, false))} \nFROM \n\t{_collection.TableNameWithSchema}";
            sql += useReadUncommitted ? " WITH (READUNCOMMITTED)" : string.Empty;

            return sql;
        }

        private List<string> GetFields(FieldsGetter fieldsGetter, bool addBrackets, bool forInsert = true)
        {
            List<string> fields = null;

            switch (fieldsGetter)
            {
                case FieldsGetter.PrimaryKeys:
                    fields = _collectionProperties.Where(x => x.Value.IsPrimaryKey).Select(x => x.Key).ToList();
                    break;

                case FieldsGetter.NotPrimaryKeys:
                    fields = _collectionProperties.Where(x => x.Value.IsPrimaryKey == false).Select(y => y.Key).ToList();
                    break;

                case FieldsGetter.NotPrimaryKeysUpdeatable:
                    fields = _collectionProperties.Where(x => x.Value.IsPrimaryKey == false).Select(y => y.Key).ToList();
                    break;

                case FieldsGetter.All:
                    fields = _collectionProperties.Where(x => x.Value.IsPrimaryKey == false || x.Value.IsPrimaryKey == true).Select(y => y.Key).ToList();
                    break;

                case FieldsGetter.AllInsertable:
                    fields = _collectionProperties.Where(x => x.Value.Identity == false).Select(y => y.Key).ToList();
                    break;

                default:
                    break;
            }

            if (fieldsGetter != FieldsGetter.PrimaryKeys && _hasModifiedDate)
                fields.Add(MODIFIED_FIELD);


            return fields;
        }

        private DataTable GetCollectionDataTable()
        {
            DataTable collectionDataTable = new DataTable(_collection.TableName);



            foreach (KeyValuePair<string, CollectionProperty> field in _collectionProperties)
            {

                collectionDataTable.Columns.Add(field.Key, Utility.ToType(field.Value.FieldType));
            }



            DataRow row;
            foreach (T item in _collection)
            {
                row = collectionDataTable.NewRow();

                int i = 0;
                foreach (KeyValuePair<string, CollectionProperty> field in _collectionProperties)
                {
                    PropertyInfo propertyInfo = field.Value.Info;
                    bool uppercase = field.Value.Uppercase;


                    if (propertyInfo.GetValue(item) == null)
                        row[field.Key] = DBNull.Value;
                    else
                    {

                        if (uppercase)
                            row[field.Key] = (propertyInfo.GetValue(item) as string).ToUpper();
                        else
                            row[field.Key] = propertyInfo.GetValue(item);
                    }


                    i++;
                }


                collectionDataTable.Rows.Add(row);
            }

            return collectionDataTable;
        }

        private Dictionary<string, IDataParameter> GetSingleItemAsParameters(T item)
        {
            Dictionary<string, IDataParameter> parameters = new Dictionary<string, IDataParameter>();



            if (_hasModifiedDate)
            {
                IDataParameter modifiedFieldParam = _dbUtility.CreateSqlParameter("@" + MODIFIED_FIELD, SqlDbType.DateTime, DateTime.UtcNow);
                parameters.Add(MODIFIED_FIELD, modifiedFieldParam);
            }

            foreach (KeyValuePair<string, CollectionProperty> field in _collectionProperties)
            {
                PropertyInfo propertyInfo = field.Value.Info;
                bool uppercase = field.Value.Uppercase;

                string fieldName = $"{field.Value.FieldName}";
                string paramName = "@" + field.Value.FieldName.Replace(" ", string.Empty);
                SqlDbType dbType = field.Value.FieldType;

                IDataParameter parameter;


                if (propertyInfo.GetValue(item) == null)
                    parameter = _dbUtility.CreateSqlParameter(paramName, dbType, DBNull.Value);
                else
                {
                    if (fieldName.Equals("IsArchiveRecord"))
                    {
                        bool isArchiveRecord = bool.Parse(propertyInfo.GetValue(item).ToString());

                        if (isArchiveRecord == true)
                        {
                            _collection.TableName = _collection.ArchiveTableName;
                        }
                    }

                    if (uppercase)
                        parameter = _dbUtility.CreateSqlParameter(paramName, dbType, (propertyInfo.GetValue(item) as string).ToUpper());
                    else if (dbType == SqlDbType.Int)
                    {
                        parameter = _dbUtility.CreateSqlParameter(paramName, dbType, (int)propertyInfo.GetValue(item));
                    }
                    else
                        parameter = _dbUtility.CreateSqlParameter(paramName, dbType, propertyInfo.GetValue(item));
                }


                parameters.Add(fieldName, parameter);

            }

            return parameters;
        }

        private async Task InsertSingleItem(IDbConnection connection, IDbTransaction transaction, T item, string partialQuery = "", List<IDataParameter> partialQueryParameters = null)
        {
            await ExecuteSingleItemAction(connection, transaction, item, true, false, false, partialQuery, partialQueryParameters);
        }
        private async Task UpdateSingleItem(IDbConnection connection, IDbTransaction transaction, T item, string partialQuery = "", List<IDataParameter> partialQueryParameters = null)
        {
            await ExecuteSingleItemAction(connection, transaction, item, false, true, false, partialQuery, partialQueryParameters);
        }
        private async Task DeleteSingleItem(IDbConnection connection, IDbTransaction transaction, T item, string partialQuery = "", List<IDataParameter> partialQueryParameters = null)
        {
            await ExecuteSingleItemAction(connection, transaction, item, false, false, true, partialQuery, partialQueryParameters);
        }

        #endregion

        #region Public

        public async Task LoadCollection()
        {
            Debug.Assert(_collection != null);

            using (IDbConnection dbConnection = await _dbUtility.GetConnection())
                await LoadCollection(dbConnection, null);
        }

        public async Task LoadCollection(IDbConnection connection, IDbTransaction transaction, bool useReadUncommitted = false)
        {
            string query = GenerateSelectQuery(useReadUncommitted);

            List<String> whereConditions = new List<string>();
            List<IDataParameter> whereParameters = new List<IDataParameter>();
            GetAdditionalWhereCondition(whereConditions, whereParameters);

            await Load(connection, transaction, query, whereConditions, whereParameters);
        }

        public async Task SaveCollection()
        {
            Debug.Assert(_collection != null);

            using (IDbConnection dbConnection = await _dbUtility.GetConnection())
                await Save(dbConnection, null);
        }

        public async Task SaveCollection(IDbConnection dbConnection, DbTransaction sqlTransaction)
        {
            Debug.Assert(dbConnection != null);
            Debug.Assert(sqlTransaction != null);

            await Save(dbConnection, sqlTransaction);
        }

        public async Task InsertSingleItem(T item)
        {
            Debug.Assert(_collection != null);

            using (IDbConnection dbConnection = await _dbUtility.GetConnection())
                await InsertSingleItem(dbConnection, null, item);
        }

        public async Task InsertSingleItem(IDbConnection dbConnection, IDbTransaction transaction, T item)
        {
            Debug.Assert(_collection != null);

            await InsertSingleItem(dbConnection, transaction, item, string.Empty, null);
        }

        public async Task UpdateSingleItem(T item)
        {
            Debug.Assert(_collection != null);

            using (IDbConnection dbConnection = await _dbUtility.GetConnection())
                await UpdateSingleItem(dbConnection, null, item);
        }

        public async Task UpdateSingleItem(IDbConnection dbConnection, IDbTransaction transaction, T item)
        {
            Debug.Assert(_collection != null);

            await UpdateSingleItem(dbConnection, transaction, item, string.Empty, null);
        }

        public async Task DeleteSingleItem(T item)
        {
            Debug.Assert(_collection != null);

            using (IDbConnection dbConnection = await _dbUtility.GetConnection())
                await DeleteSingleItem(dbConnection, null, item);
        }

        public async Task DeleteSingleItem(IDbConnection dbConnection, IDbTransaction transaction, T item)
        {
            Debug.Assert(_collection != null);

            await DeleteSingleItem(dbConnection, transaction, item, string.Empty, null);
        }

        public void Clear()
        {
            _collectionProperties.Clear();
            _collection.Clear();
        }

        public DataCollection<T> GetCollection()
        {
            return this._collection;
        }



        #endregion
    }
}
