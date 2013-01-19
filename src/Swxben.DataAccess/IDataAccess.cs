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
        void Insert<T>(T value, string tableName = null);
        void Update<T>(T value, string[] identifiers = null, string tableName = null);
        IEnumerable<T> Select<T>(string tableName = null, object where = null, string orderBy = null) where T : new();
        IEnumerable<T> Select<T>(Func<T> factory, string tableName = null, object where = null, string orderBy = null);
        IEnumerable<T> Select<T>(Func<dynamic, T> transform, string tableName = null, object where = null, string orderBy = null);
        IEnumerable<dynamic> Select(string tableName = null, object where = null, string orderBy = null);
        IEnumerable<dynamic> Select(Func<dynamic, dynamic> transform, string tableName = null, object where = null, string orderBy = null);
        Exception TestConnection();
        void DropTable(string tableName);
        bool Any<T>(object where = null);
        bool Any(Type t, object where = null);
        bool Any(string tableName, object where = null);
    }
}
