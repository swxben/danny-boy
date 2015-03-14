namespace dannyboy
{
    public partial class DataAccess
    {
        public dynamic Insert<T>(T value, string tableName = null)
        {
            using (var connection = OpenConnection())
            {
                var command = GetCommand(value);
                command.Connection = connection;
                command.CommandText = GetInsertSqlFor(typeof (T), tableName);
                command.ExecuteNonQuery();
                command.CommandText = "SELECT @@IDENTITY AS id";
                return command.ExecuteScalar();
            }
        }
    }
}