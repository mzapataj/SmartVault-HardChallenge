using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SmartVault.Shared.Interfaces;

namespace SmartVault.Shared.Data
{
    public class DbCommandService : IDbCommandService
    {
        private const string PrimaryKeyName = "Id";
        private IDbCommand command;
        private List<DbProperty> dbProperties = new List<DbProperty>();
        private Dictionary<Type, string> insertsText = new();
        private Dictionary<Type, string> updatesText = new();
        private static KeyValuePair<Type, string> cachedInsertText;
        private static KeyValuePair<Type, string> cachedUpdateText;

        public void Init(IDbCommand command)
        {
            this.command = command;
        }

        public int CreateTable<T>() where T : class
        {
            string columnsStr = "";

            var properties = typeof(T).GetProperties().ToList();
            var primaryKey = properties.FirstOrDefault(x => x.Name == PrimaryKeyName);

            if (primaryKey is not null)
                columnsStr += $"[{primaryKey.Name}] {SqlTypes.GetSqlType(primaryKey.PropertyType)} PRIMARY KEY, ";

            var otheProperties = properties.Where(x => x.Name != PrimaryKeyName);

            columnsStr += string.Join(", ", otheProperties
                .Select(x => $"[{x.Name}] {SqlTypes.GetSqlType(x.PropertyType)}"));

            var createTableStr =
                $@"CREATE TABLE [{typeof(T).Name}] (
    {columnsStr}
)";

            command.CommandText = createTableStr;
            int result = command.ExecuteNonQuery();            

            return result;
        }

        private string BuildInsertText<T>() where T : class
        {
            if (insertsText.ContainsKey(typeof(T)))
            {
                return insertsText[typeof(T)];
            }

            var properties = typeof(T).GetProperties().Select(x => x.Name);

            var parameterNames = typeof(T).GetProperties().Select(x => $"@{x.Name}{typeof(T).Name}");

            var insertStr = @$"INSERT INTO [{typeof(T).Name}] ({string.Join(", ", properties)}) VALUES ({string.Join(", ", parameterNames)})";

            dbProperties.AddRange(properties.Select(property =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{property}{typeof(T).Name}";
                command.Parameters.Add(parameter);
                return new DbProperty
                {
                    TableName = typeof(T).Name,
                    PropertyName = property,
                    DbParameter = parameter,
                };
            }).ToList());

            dbProperties.Distinct();

            insertsText.Add(typeof(T), insertStr);

            return insertStr;
        }

        private void SetInsertText<T>() where T : class
        {
            lock (this)
            {
                if (cachedInsertText.Key == typeof(T)) return;

                if (insertsText.ContainsKey(typeof(T)))
                    command.CommandText = insertsText[typeof(T)];
                else
                    command.CommandText = BuildInsertText<T>();

                cachedInsertText = new KeyValuePair<Type, string>(typeof(T), command.CommandText);
            }
        }

        public void Insert<T>(T entity) where T : class
        {
            SetInsertText<T>();
            foreach (var dbProperty in dbProperties.Where(x => x.TableName == typeof(T).Name))
            {
                var dbParameter = dbProperty.DbParameter;
                dbParameter.Value = typeof(T).GetProperty(dbProperty.PropertyName).GetValue(entity);
            }
            command.ExecuteNonQuery();
        }

        private string BuildUpdateByIdText<T>() where T : class
        {
            string whereStr = "";
            string setStr = "";

            if (insertsText.ContainsKey(typeof(T)))
            {
                return insertsText[typeof(T)];
            }

            var properties = typeof(T).GetProperties().Select(x => x.Name).ToList();
            var primaryKeyName = properties.FirstOrDefault(x => x == PrimaryKeyName);

            if (primaryKeyName is null) throw new NullReferenceException($"The class '${typeof(T).Name}' doesn't have a 'Id' parameter");

            whereStr = $"[{primaryKeyName}] = @{primaryKeyName}{typeof(T).Name}";

            var otherProperties = properties.Where(x => x != PrimaryKeyName);

            setStr = string.Join(",\n", otherProperties
                .Select(x => $"[{x}] = @{x}{typeof(T).Name}"));

            var updateStr = @$"UPDATE [{typeof(T).Name}] 
 SET {setStr}
 WHERE {whereStr}";

            dbProperties.AddRange(properties.Select(property =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{property}{typeof(T).Name}";
                command.Parameters.Add(parameter);
                return new DbProperty
                {
                    TableName = typeof(T).Name,
                    PropertyName = property,
                    DbParameter = parameter,
                };
            }).ToList());

            updatesText.Add(typeof(T), updateStr);

            return updateStr;
        }

        private void SetUpdateByIdText<T>() where T : class
        {
            lock (this)
            {
                if (cachedUpdateText.Key == typeof(T)) return;

                if (updatesText.ContainsKey(typeof(T)))
                    command.CommandText = updatesText[typeof(T)];
                else
                    command.CommandText = BuildUpdateByIdText<T>();

                cachedUpdateText = new KeyValuePair<Type, string>(typeof(T), command.CommandText);
            }
        }

        public void UpdateById<T>(T entity) where T : class
        {
            SetUpdateByIdText<T>();
            foreach (var dbProperty in dbProperties.Where(x => x.TableName == typeof(T).Name))
            {
                var dbParameter = dbProperty.DbParameter;
                dbParameter.Value = typeof(T).GetProperty(dbProperty.PropertyName).GetValue(entity);
            }
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            command.Dispose();
        }
    }

    public class DbProperty
    {
        public string TableName { get; set; }
        public string PropertyName { get; set; }
        public IDbDataParameter DbParameter { get; set; }
    }
}
