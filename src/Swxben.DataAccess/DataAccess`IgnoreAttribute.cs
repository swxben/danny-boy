using System;
using System.Reflection;

namespace swxben.dataaccess
{
    partial class DataAccess
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class IgnoreAttribute : Attribute
        {
            public static bool Test(FieldInfo field) { return Attribute.IsDefined(field, typeof(IgnoreAttribute)); }
            public static bool Test(PropertyInfo property) { return Attribute.IsDefined(property, typeof(IgnoreAttribute)); }
        }
    }
}
