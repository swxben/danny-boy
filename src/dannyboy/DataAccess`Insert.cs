using System.Data.SqlClient;
using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess
    {
        public dynamic Insert<T>(T value, string tableName = null)
        {
            using (var connection = OpenConnection())
            {
                return GetCommandForInsert(value, tableName, connection)
                    .ExecuteScalar();
            }
        }

        private static SqlCommand GetCommandForInsert<T>(T value, string tableName, SqlConnection connection)
        {
            var command = GetCommand(value);
            command.Connection = connection;
            command.CommandText = GetInsertSqlFor(typeof (T), tableName);
            command.ExecuteNonQuery();
            command.CommandText = "SELECT @@IDENTITY AS id";
            return command;
        }

        public async Task<dynamic> InsertAsync<T>(T value, string tableName = null)
        {
            using (var connection = await OpenConnectionAsync())
            {
                return await GetCommandForInsert(value, tableName, connection).ExecuteScalarAsync();
            }
        }
    }
}