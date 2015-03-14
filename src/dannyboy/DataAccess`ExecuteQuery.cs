using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess
    {
        public IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null)
        {
            using (var connection = OpenConnection())
            {
                var command = GetCommandForQuery(sql, parameters, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = GetQueryResultFromReader(reader);

                        yield return result;
                    }
                }
            }
        }

        private static ExpandoObject GetQueryResultFromReader(SqlDataReader reader)
        {
            var result = new ExpandoObject();
            var resultDictionary = result as IDictionary<string, object>;

            for (var i = 0; i < reader.FieldCount; i++)
            {
                resultDictionary.Add(reader.GetName(i), DBNull.Value.Equals(reader[i]) ? null : reader[i]);
            }
            return result;
        }

        private static SqlCommand GetCommandForQuery(string sql, object parameters, SqlConnection connection)
        {
            var command = GetCommand(parameters);
            command.Connection = connection;
            command.CommandText = sql;
            return command;
        }

        public async Task<IEnumerable<dynamic>> ExecuteQueryAsync(string sql, object parameters = null)
        {
            var results = new List<dynamic>();

            using (var connection = await OpenConnectionAsync())
            {
                var command = GetCommandForQuery(sql, parameters, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(GetQueryResultFromReader(reader));
                    }
                }
            }

            return results;
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null)
        {
            return ExecuteQuery(sql, parameters).Select(ReadDynamicIntoT<T>);
        }

        private static T ReadDynamicIntoT<T>(dynamic result)
        {
            var resultDictionary = result as IDictionary<string, object>;
            var t = Construct<T>();

            ReadResultIntoObject(resultDictionary, t);

            return t;
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object parameters = null) where T : new()
        {
            var results = await ExecuteQueryAsync(sql, parameters);

            return results.Select(ReadDynamicIntoT<T>);
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

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(Func<T> factory, string sql, object parameters = null)
        {
            var results = await ExecuteQueryAsync(sql, parameters);

            return results.Select(result =>
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

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(Func<dynamic, T> transform, string sql, object parameters = null)
        {
            var results = await ExecuteQueryAsync(sql, parameters);
            
            return results.Select(transform);
        }
    }
}