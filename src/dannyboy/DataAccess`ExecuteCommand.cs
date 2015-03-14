using System.Data.SqlClient;
using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess
    {
        public int ExecuteCommand(string sql, object parameters = null)
        {
            using (var connection = OpenConnection())
            {
                var command = GetCommandForExecution(sql, parameters, connection);

                return command.ExecuteNonQuery();
            }
        }

        public async Task<int> ExecuteCommandAsync(string sql, object parameters = null)
        {
            using (var connection = await OpenConnectionAsync())
            {
                var command = GetCommandForExecution(sql, parameters, connection);
                
                return await command.ExecuteNonQueryAsync();
            }
        }

        private static SqlCommand GetCommandForExecution(string sql, object parameters, SqlConnection connection)
        {
            var command = GetCommand(parameters);
            command.Connection = connection;
            command.CommandText = sql;
            return command;
        }
    }
}