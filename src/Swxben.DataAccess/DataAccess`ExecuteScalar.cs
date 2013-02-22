using System.Collections.Generic;
using System.Linq;

namespace swxben.dataaccess
{
    partial class DataAccess
    {
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
