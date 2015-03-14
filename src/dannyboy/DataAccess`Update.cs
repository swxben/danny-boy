using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess
    {
        public void Update<T>(T value, string[] identifiers = null, string tableName = null)
        {
            var sql = GetUpdateSqlFor(typeof (T), identifiers, tableName);
            ExecuteCommand(sql, value);
        }

        public Task UpdateAsync<T>(T value, string[] identifiers = null, string tableName = null)
        {
            var sql = GetUpdateSqlFor(typeof (T), identifiers, tableName);
            return ExecuteCommandAsync(sql, value);
        }
    }
}