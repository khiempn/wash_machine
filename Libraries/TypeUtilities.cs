using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Libraries
{
    public static class TypeUtilities
    {
        public static IEnumerable<FieldInfo> GetConstants(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        }

        public static IEnumerable<string> GetConstantsValues(this Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var fileds = fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
            var result = fieldInfos.Select(c => c.GetValue(type).ToString());
            return result;
        }

        public static IEnumerable<T> GetConstantsValues<T>(this Type type) where T : class
        {
            var fieldInfos = GetConstants(type);

            return fieldInfos.Select(fi => fi.GetRawConstantValue() as T);
        }
    }
}
