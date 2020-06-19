using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Excel.File.Service.Service.Extensions
{
    public static class StringExtensions
    {
        public static object Parse(this string value, Type type)
            => ParseMethod.MakeGenericMethod(type).Invoke(null, new object[] { value });

        public static T Parse<T>(this string value)
           => (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);

        static readonly MethodInfo ParseMethod = typeof(StringExtensions)
                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
                   .Single(t => t.IsGenericMethod && t.Name == "Parse");
    }
}
