using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess
    {
        public void Truncate(string tableName)
        {
            ExecuteCommand(GetTruncateSql(tableName));
        }

        private static string GetTruncateSql(string tableName)
        {
            var sql = string.Format("TRUNCATE TABLE [{0}]", tableName);
            return sql;
        }

        public Task TruncateAsync(string tableName)
        {
            return ExecuteCommandAsync(GetTruncateSql(tableName));
        }

        public void Truncate<T>()
        {
            Truncate(GetTableName(typeof(T)));
        }

        public Task TruncateAsync<T>()
        {
            return TruncateAsync(GetTableName(typeof (T)));
        }
    }
}