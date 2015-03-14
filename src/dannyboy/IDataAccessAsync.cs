using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dannyboy
{
    public interface IDataAccessAsync
    {
        Task<int> ExecuteCommandAsync(string sql, object parameters = null);
        Task<IEnumerable<dynamic>> ExecuteQueryAsync(string sql, object parameters = null);
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, object parameters = null);
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(Func<T> factory, string sql, object parameters = null);
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(Func<dynamic, T> transform, string sql, object parameters = null);
        Task<dynamic> InsertAsync<T>(T value, string tableName = null);
        Task UpdateAsync<T>(T value, string[] identifiers = null, string tableName = null);
        Task<IEnumerable<T>> SelectAsync<T>(string tableName = null, object where = null, string orderBy = null);
        Task<IEnumerable<T>> SelectAsync<T>(Func<T> factory, string tableName = null, object where = null, string orderBy = null);
        Task<IEnumerable<T>> SelectAsync<T>(Func<dynamic, T> transform, string tableName = null, object where = null, string orderBy = null);
        Task<IEnumerable<dynamic>> SelectAsync(string tableName = null, object where = null, string orderBy = null);
        Task<IEnumerable<dynamic>> SelectAsync(Func<dynamic, dynamic> transform, string tableName = null, object where = null, string orderBy = null);
        Task<Exception> TestConnectionAsync();
        Task DropTableAsync(string tableName);
        Task<bool> TableExistsAsync(string tableName);
        Task<bool> AnyAsync<T>(object @where = null);
        Task<bool> AnyAsync(Type t, object @where = null);
        Task<bool> AnyAsync(string tableName, object @where = null);
        Task<bool> ExistsAsync<T>(object @where = null);
        Task<bool> ExistsAsync(Type t, object @where = null);
        Task<bool> ExistsAsync(string tableName, object @where = null);
        string GetDatabaseName();
        Task<object> ExecuteScalarAsync(string sql, object parameters = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null);
    }
}