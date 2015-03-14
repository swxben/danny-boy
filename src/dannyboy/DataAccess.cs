using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess : IDataAccess
    {
        readonly string _connectionString = "";

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetDatabaseName()
        {
            if (_connectionString.Contains("Initial Catalog"))
            {
                var db = _connectionString.Split(';').FirstOrDefault(s => s.Trim().StartsWith("Initial Catalog="));
                return db == null ? "" : db.Trim().Replace("Initial Catalog=", "");
            }
            if (_connectionString.Contains("Database"))
            {
                var db = _connectionString.Split(';').FirstOrDefault(s => s.Trim().StartsWith("Database="));
                return db == null ? "" : db.Trim().Replace("Database=", "");
            }
            return "";
        }

        async Task<SqlConnection> OpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            return connection;
        }

        SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);

            connection.Open();

            return connection;
        }

        public Exception TestConnection()
        {
            try
            {
                using (OpenConnection())
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task<Exception> TestConnectionAsync()
        {
            try
            {
                using (await OpenConnectionAsync())
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public void DropTable(string tableName)
        {
            var sql = GetDropTableSql(tableName);

            ExecuteCommand(sql);
        }

        private static string GetDropTableSql(string tableName)
        {
            var sql = string.Format(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN (N'U'))
    DROP TABLE {0}", tableName);
            return sql;
        }

        public Task DropTableAsync(string tableName)
        {
            return ExecuteCommandAsync(GetDropTableSql(tableName));
        }

        public bool TableExists(string tableName)
        {
            var query = GetTableExistsSql(tableName);

            return ExecuteQuery(query).Any();
        }

        private static string GetTableExistsSql(string tableName)
        {
            var query = string.Format(
                "SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN (N'U')",
                tableName);
            return query;
        }

        public async Task<bool> TableExistsAsync(string tableName)
        {
            var result = await ExecuteQueryAsync(GetTableExistsSql(tableName));
            
            return result.Any();
        }
    }
}
