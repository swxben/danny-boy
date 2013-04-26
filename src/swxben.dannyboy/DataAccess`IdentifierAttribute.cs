using System;
using System.Reflection;

namespace swxben.dannyboy
{
    partial class DataAccess
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class IdentifierAttribute : Attribute
        {
            public static bool Test(FieldInfo field) { return Attribute.IsDefined(field, typeof(IdentifierAttribute)); }
            public static bool Test(PropertyInfo property) { return Attribute.IsDefined(property, typeof(IdentifierAttribute)); }
        }
    }
}
