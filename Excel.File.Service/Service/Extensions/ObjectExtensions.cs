﻿using System.Reflection;

namespace Excel.File.Service.Service.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetValueOrDefault(this PropertyInfo property, object target, object defaultValue)
            => property.GetValue(target) ?? defaultValue;
    }
}
