using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace swxben.dataaccess
{
    internal class DataAccessSqlGeneration
    {
        public static object GetValue(object value, Type type)
        {
            if (value == null) return null;
            if (!(value is string)) return value;
            if (type.IsEnum) return Enum.Parse(type, value as string);

            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType == null) return value;
            if (underlyingType.IsEnum) return Enum.Parse(underlyingType, value as string);

            return value;
        }

        public static SqlCommand GetCommand(string sql, SqlConnection connection, object parameters)
        {
            var command = new SqlCommand();

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;
            command.CommandText = sql;

            if (parameters != null)
            {
                var properties = parameters.GetType().GetProperties()
                    .Where(p => !DataAccess.IgnoreAttribute.Test(p));
                var fields = parameters.GetType().GetFields()
                    .Where(f => !DataAccess.IgnoreAttribute.Test(f));
                foreach (var property in properties)
                {
                    var value = property.GetValue(parameters, null);
                    AddParameterValueToCommand(command, property.Name, value);
                }
                foreach (var field in fields)
                {
                    var value = field.GetValue(parameters);
                    AddParameterValueToCommand(command, field.Name, value);
                }
            }

            return command;
        }

        public static void AddParameterValueToCommand(SqlCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", name);

            if (value != null && value.GetType().IsEnum) value = value.ToString();

            parameter.Value = value == null ? DBNull.Value : value;

            if (value != null && value.GetType() == typeof(string))
            {
                parameter.Size = ((string)value).Length > 4000 ? -1 : 4000;
            }

            command.Parameters.Add(parameter);
        }

        public static string GetInsertSqlFor<T>()
        {
            var fieldsSql = new StringBuilder();
            var valuesSql = new StringBuilder();
            foreach (var field in GetAllFieldNames<T>())
            {
                if (!string.IsNullOrEmpty(fieldsSql.ToString()))
                {
                    fieldsSql.Append(", ");
                    valuesSql.Append(", ");
                }
                fieldsSql.Append(field);
                valuesSql.Append("@").Append(field);
            }

            return string.Format(
                "INSERT INTO {0}s({1}) VALUES({2})",
                typeof(T).Name,
                fieldsSql,
                valuesSql);
        }

        public static string GetUpdateSqlFor<T>(params string[] identifiers)
        {
            var set = new StringBuilder();

            foreach (var field in GetAllFieldNames<T>())
            {
                if (identifiers.Any(id => string.Equals(id, field, StringComparison.InvariantCultureIgnoreCase))) continue;
                if (!string.IsNullOrEmpty(set.ToString())) set.Append(", ");
                set.AppendFormat("{0} = @{0}", field);
            }

            var identifiersCondition = new StringBuilder();
            foreach (var id in identifiers) identifiersCondition.AppendFormat(" AND {0} = @{0}", id);

            return string.Format(
                "UPDATE {0}s SET {1} WHERE 1=1 {2}",
                typeof(T).Name,
                set,
                identifiersCondition);
        }

        public static string GetUpdateSqlFor<T>()
        {
            return GetUpdateSqlFor<T>(GetIdentifiers<T>().ToArray());
        }

        static IEnumerable<string> GetIdentifiers<T>()
        {
            var fields = typeof(T).GetFields().Where(f => DataAccess.IdentifierAttribute.Test(f)).Select(f => f.Name);
            var properties = typeof(T).GetProperties().Where(p => DataAccess.IdentifierAttribute.Test(p)).Select(p => p.Name);
            return fields.Concat(properties);
        }

        public static IEnumerable<string> GetAllFieldNames<T>()
        {
            return GetAllFieldNames(typeof(T));
        }

        public static IEnumerable<string> GetAllFieldNames(Type t)
        {
            var fieldNames = t.GetFields().Where(f => !DataAccess.IgnoreAttribute.Test(f)).Select(f => f.Name);
            var propertyNames = t.GetProperties().Where(p => !DataAccess.IgnoreAttribute.Test(p)).Select(p => p.Name);
            return fieldNames.Concat(propertyNames);
        }

        public static string GetSelectSqlFor<T>(object criteria = null, string orderBy = null)
        {
            var where = new StringBuilder();
            if (criteria != null)
            {
                where.Append(" WHERE 1=1 ");

                foreach (var field in GetAllFieldNames(criteria.GetType()))
                {
                    where.AppendFormat(" AND {0} = @{0}", field);
                }
            }

            orderBy = string.IsNullOrEmpty(orderBy) ? "" : string.Format("ORDER BY {0}", orderBy);

            return string.Format(
                "SELECT * FROM {0}s {1} {2}",
                typeof(T).Name,
                where,
                orderBy);
        }
    }
}
