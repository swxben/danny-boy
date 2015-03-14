using System;
using System.Threading.Tasks;

namespace dannyboy
{
    public partial class DataAccess
    {
        public void Delete<T>(object @where, string tableName = null)
        {
            ExecuteCommand(
                GetDeleteSqlFor(typeof (T), @where, tableName),
                @where);
        }

        public void Delete(Type t, object @where, string tableName = null)
        {
            ExecuteCommand(
                GetDeleteSqlFor(t, @where, tableName),
                @where);
        }

        public void Delete(object @where, string tableName)
        {
            ExecuteCommand(
                GetDeleteSqlFor(null, @where, tableName),
                @where);
        }

        public void Delete(object instance)
        {
            ExecuteCommand(
                GetDeleteSqlFor(instance), 
                instance);
        }

        public Task DeleteAsync<T>(object @where, string tableName = null)
        {
            return ExecuteCommandAsync(
                GetDeleteSqlFor(typeof (T), @where, tableName),
                @where);
        }

        public Task DeleteAsync(Type t, object @where, string tableName = null)
        {
            return ExecuteCommandAsync(
                GetDeleteSqlFor(t, @where, tableName),
                @where);
        }

        public Task DeleteAsync(object @where, string tableName)
        {
            return ExecuteCommandAsync(
                GetDeleteSqlFor(null, @where, tableName),
                @where);
        }

        public Task DeleteAsync(object instance)
        {
            return ExecuteCommandAsync(
                GetDeleteSqlFor(instance), 
                instance);
        }
    }
}