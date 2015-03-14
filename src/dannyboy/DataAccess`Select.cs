using System;
using System.Collections.Generic;

namespace dannyboy
{
    public partial class DataAccess
    {
        public IEnumerable<T> Select<T>(
            Func<dynamic, T> transform,
            string tableName = null,
            object where = null,
            string orderBy = null)
        {
            var sql = GetSelectSqlFor(typeof (T), where, orderBy, tableName);
            return ExecuteQuery(transform, sql, where);
        }

        public IEnumerable<dynamic> Select(
            string tableName = null,
            object where = null,
            string orderBy = null)
        {
            var sql = GetSelectSqlFor(null, where, orderBy, tableName);
            return ExecuteQuery(sql, where);
        }

        public IEnumerable<dynamic> Select(
            Func<dynamic, dynamic> transform,
            string tableName = null,
            object where = null,
            string orderBy = null)
        {
            var sql = GetSelectSqlFor(null, where, orderBy, tableName);
            return ExecuteQuery(transform, sql, where);
        }

        public IEnumerable<T> Select<T>(
            string tableName = null,
            object where = null,
            string orderBy = null)
        {
            var sql = GetSelectSqlFor(typeof (T), where, orderBy, tableName);
            return ExecuteQuery<T>(sql, where);
        }

        public IEnumerable<T> Select<T>(
            Func<T> factory,
            string tableName = null,
            object where = null,
            string orderBy = null)
        {
            var sql = GetSelectSqlFor(typeof (T), where, orderBy, tableName);
            return ExecuteQuery(factory, sql, where);
        }
    }
}