using System;
using System.Collections.Generic;

namespace swxben.dataaccess
{
    public interface IDataAccess
    {
        int ExecuteCommand(string sql, object parameters = null);
        IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null);
        IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null) where T : new();
        void Insert<T>(T value);
        void Update<T>(T value, string id);
        void DropTable(string tableName);
        IEnumerable<T> Select<T>(
            object where = null,
            string orderBy = null
            ) where T : new();
        Exception TestConnection();
    }
}
