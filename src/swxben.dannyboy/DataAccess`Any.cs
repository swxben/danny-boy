using System;
using System.Linq;

namespace swxben.dannyboy
{
    partial class DataAccess
    {
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

        public bool Exists<T>(object where = null) { return Any<T>(where); }

        public bool Exists(Type t, object where = null) { return Any(t, where); }

        public bool Exists(string tableName, object where = null) { return Any(tableName, where); }
    }
}
