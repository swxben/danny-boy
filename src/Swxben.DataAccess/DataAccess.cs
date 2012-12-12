﻿using System;
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

                var properties = typeof(T)
                    .GetProperties()
                    .Where(p => resultDictionary.ContainsKey(p.Name))
                    .Where(p => p.CanWrite)
                    .Where(p => !IgnoreAttribute.Test(p));
                var fields = typeof(T)
                    .GetFields()
                    .Where(f => resultDictionary.ContainsKey(f.Name))
                    .Where(f => !IgnoreAttribute.Test(f));

                foreach (var property in properties)
                {
                    property.SetValue(t, DataAccessSqlGeneration.GetValue(resultDictionary[property.Name], property.PropertyType), null);
                }
                foreach (var field in fields)
                {
                    field.SetValue(t, DataAccessSqlGeneration.GetValue(resultDictionary[field.Name], field.FieldType));
                }

                return t;
            });
        }

        public void Insert<T>(T value)
        {
            ExecuteCommand(GetInsertSqlFor<T>(), value);
        }

        public void Update<T>(T value, string id)
        {
            ExecuteCommand(GetUpdateSqlFor<T>(id), value);
        }


        public IEnumerable<T> Select<T>(
            object where = null,
            string orderBy = null
            ) where T : new()
        {
            var sql = GetSelectSqlFor<T>(where, orderBy);
            return ExecuteQuery<T>(sql, where);
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

        public static string GetUpdateSqlFor<T>(string id)
        {
            return DataAccessSqlGeneration.GetUpdateSqlFor<T>(id);
        }

        public static string GetSelectSqlFor<T>(object criteria = null, string orderBy = null)
        {
            return DataAccessSqlGeneration.GetSelectSqlFor<T>(criteria, orderBy);
        }
    }

}
