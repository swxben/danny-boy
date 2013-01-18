using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace swxben.dataaccess
{
    public class DataAccess : IDataAccess
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class IgnoreAttribute : Attribute
        {
            public static bool Test(FieldInfo field) { return Attribute.IsDefined(field, typeof(IgnoreAttribute)); }
            public static bool Test(PropertyInfo property) { return Attribute.IsDefined(property, typeof(IgnoreAttribute)); }
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class IdentifierAttribute : Attribute
        {
            public static bool Test(FieldInfo field) { return Attribute.IsDefined(field, typeof(IdentifierAttribute)); }
            public static bool Test(PropertyInfo property) { return Attribute.IsDefined(property, typeof(IdentifierAttribute)); }
        }

        string _connectionString = "";

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int ExecuteCommand(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = DataAccessSqlGeneration.GetCommand(sql, connection, parameters);

                connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        public IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = DataAccessSqlGeneration.GetCommand(sql, connection, parameters);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new ExpandoObject();
                        var resultDictionary = result as IDictionary<string, object>;

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            resultDictionary.Add(reader.GetName(i), DBNull.Value.Equals(reader[i]) ? null : reader[i]);
                        }

                        yield return result;
                    }
                }
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null) where T : new()
        {
            return ExecuteQuery(sql, parameters).Select(result =>
            {
                var resultDictionary = result as IDictionary<string, object>;
                var t = new T();

                ReadResultIntoObject<T>(resultDictionary, t);

                return t;
            });
        }

        public IEnumerable<T> ExecuteQuery<T>(Func<T> factory, string sql, object parameters = null)
        {
            return ExecuteQuery(sql, parameters).Select(result =>
            {
                var resultDictionary = result as IDictionary<string, object>;
                var t = factory();

                ReadResultIntoObject<T>(resultDictionary, t);

                return t;
            });
        }

        public IEnumerable<T> ExecuteQuery<T>(Func<dynamic, T> transform, string sql, object parameters = null)
        {
            return ExecuteQuery(sql, parameters).Select(transform);
        }

        private static void ReadResultIntoObject<T>(IDictionary<string, object> resultDictionary, T t)
        {
            var properties = typeof(T)
                .GetProperties()
                .Where(p => resultDictionary.ContainsKey(p.Name))
                .Where(p => p.CanWrite);
            var fields = typeof(T)
                .GetFields()
                .Where(f => resultDictionary.ContainsKey(f.Name));

            foreach (var property in properties)
            {
                property.SetValue(t, DataAccessSqlGeneration.GetValue(resultDictionary[property.Name], property.PropertyType), null);
            }
            foreach (var field in fields)
            {
                field.SetValue(t, DataAccessSqlGeneration.GetValue(resultDictionary[field.Name], field.FieldType));
            }
        }

        public void Insert<T>(T value)
        {
            ExecuteCommand(GetInsertSqlFor<T>(), value);
        }

        public void Update<T>(T value, params string[] identifiers)
        {
            ExecuteCommand(GetUpdateSqlFor<T>(identifiers), value);
        }

        public IEnumerable<T> Select<T>(
            object where = null,
            string orderBy = null
            ) where T : new()
        {
            return ExecuteQuery<T>(GetSelectSqlFor<T>(where, orderBy), where);
        }

        public IEnumerable<T> Select<T>(
            Func<T> factory,
            object where = null,
            string orderBy = null)
        {
            return ExecuteQuery(factory, GetSelectSqlFor<T>(where, orderBy), where);
        }

        public IEnumerable<T> Select<T>(
            Func<dynamic, T> transform,
            object where = null,
            string orderBy = null)
        {
            return ExecuteQuery(transform, GetSelectSqlFor<T>(where, orderBy), where);
        }

        public void DropTable(string tableName)
        {
            var sql = string.Format(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN (N'U'))
    DROP TABLE {0}", tableName);

            ExecuteCommand(sql);
        }

        public Exception TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static string GetInsertSqlFor<T>()
        {
            return DataAccessSqlGeneration.GetInsertSqlFor<T>();
        }

        public static string GetUpdateSqlFor<T>(params string[] identifiers)
        {
            if (identifiers.Any()) return DataAccessSqlGeneration.GetUpdateSqlFor<T>(identifiers);
            return DataAccessSqlGeneration.GetUpdateSqlFor<T>();
        }

        public static string GetSelectSqlFor<T>(object criteria = null, string orderBy = null)
        {
            return DataAccessSqlGeneration.GetSelectSqlFor<T>(criteria, orderBy);
        }

        public bool Any<T>(object criteria = null)
        {
            var sql = string.Format(
                "SELECT 1 FROM {0} {1}", 
                DataAccessSqlGeneration.GetTableName<T>(), 
                DataAccessSqlGeneration.GetWhereForCriteria(criteria));
            return ExecuteQuery(sql, criteria).Any();
        }
    }
}
