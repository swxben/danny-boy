﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace swxben.dataaccess
{
    public partial class DataAccess : IDataAccess
    {
        readonly string _connectionString = "";

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetDatabaseName()
        {
            if (_connectionString.Contains("Initial Catalog"))
            {
                var db = _connectionString.Split(';').FirstOrDefault(s => s.Trim().StartsWith("Initial Catalog="));
                return db == null ? "" : db.Trim().Replace("Initial Catalog=", "");
            }
            if (_connectionString.Contains("Database"))
            {
                var db = _connectionString.Split(';').FirstOrDefault(s => s.Trim().StartsWith("Database="));
                return db == null ? "" : db.Trim().Replace("Database=", "");
            }
            return "";
        }

        public int ExecuteCommand(string sql, object parameters = null)
        {
            using (var connection = OpenConnection())
            {
                var command = GetCommand(parameters);
                command.Connection = connection;
                command.CommandText = sql;

                return command.ExecuteNonQuery();
            }
        }

        SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null)
        {
            using (var connection = OpenConnection())
            {
                var command = GetCommand(parameters);
                command.Connection = connection;
                command.CommandText = sql;

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

                ReadResultIntoObject(resultDictionary, t);

                return t;
            });
        }

        public IEnumerable<T> ExecuteQuery<T>(Func<T> factory, string sql, object parameters = null)
        {
            return ExecuteQuery(sql, parameters).Select(result =>
            {
                var resultDictionary = result as IDictionary<string, object>;
                var t = factory();

                ReadResultIntoObject(resultDictionary, t);

                return t;
            });
        }

        public IEnumerable<T> ExecuteQuery<T>(Func<dynamic, T> transform, string sql, object parameters = null)
        {
            return ExecuteQuery(sql, parameters).Select(transform);
        }

        public dynamic Insert<T>(T value, string tableName = null)
        {
            using (var connection = OpenConnection())
            {
                var command = GetCommand(value);
                command.Connection = connection;
                command.CommandText = GetInsertSqlFor(typeof(T), tableName);
                command.ExecuteNonQuery();
                command.CommandText = "SELECT @@IDENTITY AS id";
                return command.ExecuteScalar();
            }
        }

        public void Update<T>(T value, string[] identifiers = null, string tableName = null)
        {
            var sql = GetUpdateSqlFor(typeof(T), identifiers, tableName);
            ExecuteCommand(sql, value);
        }

        public IEnumerable<T> Select<T>(
            string tableName = null,
            object where = null,
            string orderBy = null
            ) where T : new()
        {
            var sql = GetSelectSqlFor(typeof(T), where, orderBy, tableName);
            return ExecuteQuery<T>(sql, where);
        }

        public IEnumerable<T> Select<T>(
            Func<T> factory,
            string tableName = null,
            object where = null,
            string orderBy = null
            )
        {

            var sql = GetSelectSqlFor(typeof(T), where, orderBy, tableName);
            return ExecuteQuery(factory, sql, where);
        }

        public IEnumerable<T> Select<T>(
            Func<dynamic, T> transform,
            string tableName = null,
            object where = null,
            string orderBy = null
            )
        {
            var sql = GetSelectSqlFor(typeof(T), where, orderBy, tableName);
            return ExecuteQuery(transform, sql, where);
        }

        public IEnumerable<dynamic> Select(
            string tableName = null,
            object where = null,
            string orderBy = null
            )
        {
            var sql = GetSelectSqlFor(null, where, orderBy, tableName);
            return ExecuteQuery(sql, where);
        }

        public IEnumerable<dynamic> Select(
            Func<dynamic, dynamic> transform,
            string tableName = null,
            object where = null,
            string orderBy = null)
        {
            var sql = GetSelectSqlFor(null, where, orderBy, tableName);
            return ExecuteQuery(transform, sql, where);
        }

        public Exception TestConnection()
        {
            try
            {
                using (OpenConnection())
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public void DropTable(string tableName)
        {
            var sql = string.Format(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN (N'U'))
    DROP TABLE {0}", tableName);

            ExecuteCommand(sql);
        }

        public bool Any<T>(object where = null)
        {
            return Any(typeof(T), where);
        }

        public bool Any(Type t, object where = null)
        {
            return Any(GetTableName(t), where);
        }

        public bool Any(string tableName, object where = null)
        {
            var sql = string.Format(
                "SELECT 1 FROM {0} {1}",
                tableName,
                GetWhereForCriteria(where));
            return ExecuteQuery(sql, where).Any();
        }

        public object ExecuteScalar(string sql, object parameters)
        {
            return ((IDictionary<string, object>)ExecuteQuery(sql, parameters).First()).Values.First();
        }

        public T ExecuteScalar<T>(string sql, object parameters)
        {
            return (T)ExecuteScalar(sql, parameters);
        }
    }
}
