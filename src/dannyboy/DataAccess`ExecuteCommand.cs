using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess
    {
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

        public Task<int> ExecuteCommandAsync(string sql, object parameters = null)
        {
            return Task.FromResult(ExecuteCommand(sql, parameters));
        }
    }
}