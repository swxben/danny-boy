using System;
using System.Linq;
using System.Threading.Tasks;

namespace dannyboy
{
    partial class DataAccess
    {
        public bool Any<T>(object where = null)
        {
            return Any(typeof(T), where);
        }

        public Task<bool> AnyAsync<T>(object where = null)
        {
            return AnyAsync(typeof (T), where);
        }

        public bool Any(Type t, object where = null)
        {
            return Any(GetTableName(t), where);
        }

        public Task<bool> AnyAsync(Type t, object where = null)
        {
            return AnyAsync(GetTableName(t), where);
        }

        public bool Any(string tableName, object where = null)
        {
            var sql = GetAnySelectSql(tableName, @where);
            return ExecuteQuery(sql, where).Any();
        }

        private static string GetAnySelectSql(string tableName, object @where)
        {
            var sql = string.Format(
                "SELECT 1 FROM {0} {1}",
                tableName,
                GetWhereForCriteria(@where));
            return sql;
        }

        public async Task<bool> AnyAsync(string tableName, object where = null)
        {
            var result = await ExecuteQueryAsync(GetAnySelectSql(tableName, @where), @where);
            
            return result.Any();
        }

        public bool Exists<T>(object where = null) { return Any<T>(where); }

        public Task<bool> ExistsAsync<T>(object @where = null)
        {
            return AnyAsync<T>(@where);
        }

        public bool Exists(Type t, object where = null) { return Any(t, where); }

        public Task<bool> ExistsAsync(Type t, object @where = null)
        {
            return AnyAsync(t, @where);
        }

        public bool Exists(string tableName, object where = null) { return Any(tableName, where); }

        public Task<bool> ExistsAsync(string tableName, object @where = null)
        {
            return AnyAsync(tableName, @where);
        }
    }
}
