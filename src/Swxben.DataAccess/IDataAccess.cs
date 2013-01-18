using System;
using System.Collections.Generic;

namespace swxben.dataaccess
{
    public interface IDataAccess
    {
        int ExecuteCommand(string sql, object parameters = null);
        IEnumerable<dynamic> ExecuteQuery(string sql, object parameters = null);
        IEnumerable<T> ExecuteQuery<T>(string sql, object parameters = null) where T : new();
        IEnumerable<T> ExecuteQuery<T>(Func<T> factory, string sql, object parameters = null);
        IEnumerable<T> ExecuteQuery<T>(Func<dynamic, T> transform, string sql, object parameters = null);
        void Insert<T>(T value);
        void Update<T>(T value, params string[] identifiers);
        void DropTable(string tableName);
        IEnumerable<T> Select<T>(object where = null, string orderBy = null) where T : new();
        IEnumerable<T> Select<T>(Func<T> factory, object where = null, string orderBy = null);
        IEnumerable<T> Select<T>(Func<dynamic, T> transform, object where = null, string orderBy = null);
        Exception TestConnection();
        bool Any<T>(object where = null);
    }
}
