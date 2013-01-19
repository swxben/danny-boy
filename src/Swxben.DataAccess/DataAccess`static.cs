using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace swxben.dataaccess
{
    partial class DataAccess
    {
        private static void ReadResultIntoObject<T>(IDictionary<string, object> resultDictionary, T t)
        {
            var properties = typeof(T)
                .GetProperties()
                .Where(p => resultDictionary.ContainsKey(p.Name))
                .Where(p => p.CanWrite);
            var fields = typeof(T)
                .GetFields()
                .Where(f => resultDictionary.ContainsKey(f.Name));

            foreach (var property in properties)
            {
                property.SetValue(t, GetValue(resultDictionary[property.Name], property.PropertyType), null);
            }
            foreach (var field in fields)
            {
                field.SetValue(t, GetValue(resultDictionary[field.Name], field.FieldType));
            }
        }

        static object GetValue(object value, Type type)
        {
            if (value == null) return null;
            if (!(value is string)) return value;
            if (type.IsEnum) return Enum.Parse(type, value as string);

            var underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType == null) return value;
            if (underlyingType.IsEnum) return Enum.Parse(underlyingType, value as string);

            return value;
        }

        static SqlCommand GetCommand(string sql, SqlConnection connection, object parameters)
        {
            var command = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text, 
                    Connection = connection, 
                    CommandText = sql
                };

            if (parameters != null)
            {
                var properties = parameters.GetType().GetProperties()
                    .Where(p => !IgnoreAttribute.Test(p));
                var fields = parameters.GetType().GetFields()
                    .Where(f => !IgnoreAttribute.Test(f));
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

        static void AddParameterValueToCommand(SqlCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", name);

            if (value != null && value.GetType().IsEnum) value = value.ToString();

            parameter.Value = value ?? DBNull.Value;

            var s = value as string;
            if (s != null)
            {
                parameter.Size = s.Length > 4000 ? -1 : 4000;
            }

            command.Parameters.Add(parameter);
        }

        public static string GetInsertSqlFor(Type t, string tableName = null)
        {
            var fieldsSql = new StringBuilder();
            var valuesSql = new StringBuilder();
            foreach (var field in GetAllFieldNames(t))
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
                "INSERT INTO {0}({1}) VALUES({2})",
                tableName ?? GetTableName(t),
                fieldsSql,
                valuesSql);
        }

        public static string GetUpdateSqlFor(Type t, string[] identifiers = null, string tableName = null)
        {
            var set = new StringBuilder();
            identifiers = identifiers ?? GetIdentifiers(t).ToArray();
            tableName = tableName ?? GetTableName(t);

            foreach (var field in GetAllFieldNames(t))
            {
                if (identifiers.Any(id => string.Equals(id, field, StringComparison.InvariantCultureIgnoreCase))) continue;
                if (!string.IsNullOrEmpty(set.ToString())) set.Append(", ");
                set.AppendFormat("{0} = @{0}", field);
            }

            var identifiersCondition = new StringBuilder();
            foreach (var id in identifiers) identifiersCondition.AppendFormat(" AND {0} = @{0}", id);

            return string.Format(
                "UPDATE {0} SET {1} WHERE 1=1 {2}",
                tableName,
                set,
                identifiersCondition);
        }

        static IEnumerable<string> GetIdentifiers(Type t)
        {
            var fields = t.GetFields().Where(IdentifierAttribute.Test).Select(f => f.Name);
            var properties = t.GetProperties().Where(IdentifierAttribute.Test).Select(p => p.Name);
            return fields.Concat(properties);
        }

        static IEnumerable<string> GetAllFieldNames(Type t)
        {
            var fieldNames = t.GetFields().Where(f => !IgnoreAttribute.Test(f)).Select(f => f.Name);
            var propertyNames = t.GetProperties().Where(p => !IgnoreAttribute.Test(p)).Select(p => p.Name);
            return fieldNames.Concat(propertyNames);
        }

        public static string GetSelectSqlFor(Type t, object criteria, string orderBy, string tableName)
        {
            tableName = tableName ?? GetTableName(t);
            var wherePart = GetWhereForCriteria(criteria);
            var orderByPart = string.IsNullOrEmpty(orderBy) ? "" : string.Format("ORDER BY {0}", orderBy);

            return string.Format(
                "SELECT * FROM {0} {1} {2}",
                tableName,
                wherePart,
                orderByPart);
        }

        static string GetTableName(Type t)
        {
            return t.Name + "s";
        }

        static string GetWhereForCriteria(object criteria)
        {
            var wherePart = new StringBuilder();
            if (criteria != null)
            {
                wherePart.Append(" WHERE 1=1 ");

                foreach (var field in GetAllFieldNames(criteria.GetType()))
                {
                    wherePart.AppendFormat(" AND {0} = @{0}", field);
                }
            }
            return wherePart.ToString();
        }

    }
}
