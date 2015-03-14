using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dannyboy
{
    partial class DataAccess
    {
        public object ExecuteScalar(string sql, object parameters)
        {
            return ((IDictionary<string, object>)ExecuteQuery(sql, parameters).First()).Values.First();
        }

        public async Task<object> ExecuteScalarAsync(string sql, object parameters)
        {
            var result = await ExecuteQueryAsync(sql, parameters);
            return ((IDictionary<string, object>)result.First()).Values.First();

        }

        public T ExecuteScalar<T>(string sql, object parameters)
        {
            return (T)ExecuteScalar(sql, parameters);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object parameters)
        {
            var result = await ExecuteScalarAsync(sql, parameters);

            return (T) result;
        }
    }
}
