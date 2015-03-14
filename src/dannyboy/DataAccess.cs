using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

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

        SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public void Update<T>(T value, string[] identifiers = null, string tableName = null)
        {
            var sql = GetUpdateSqlFor(typeof(T), identifiers, tableName);
            ExecuteCommand(sql, value);
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

        public void DropTable(string tableName)
        {
            var sql = string.Format(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN (N'U'))
    DROP TABLE {0}", tableName);

            ExecuteCommand(sql);
        }

        public bool TableExists(string tableName)
        {
            var query = string.Format(
                "SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type IN (N'U')",
                tableName);
            
            return ExecuteQuery(query).Any();
        }
    }
}
