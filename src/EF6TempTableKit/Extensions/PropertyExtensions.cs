using EF6TempTableKit.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EF6TempTableKit.Extensions
{
    internal static class PropertyExtensions
    {
        public static object GetSqlValue(this PropertyInfo prop, object obj, IDictionary<string, Attribute[]> customFormatter)
        {
            var hasStringCustomFormatter = customFormatter[prop.Name]?.Length > 0;
            var hasFuncCustomFormatter = customFormatter[prop.Name]?.Length > 0;


            var value = prop.GetValue(obj, null);

            if (prop.PropertyType == typeof(bool))
            {
                value = (bool)value ? 1 : 0;
            }
            else if (prop.PropertyType == typeof(string))
            {
                value = value.WrapWithSingleQuotes();
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff").WrapWithSingleQuotes();
            }

            return value;
        }

        private static object DefaultFormatValue(PropertyInfo prop, object obj) 
        {
            var value = prop.GetValue(obj, null);

            if (prop.PropertyType == typeof(bool))
            {
                value = (bool)value ? 1 : 0;
            }
            else if (prop.PropertyType == typeof(string))
            {
                value = value.WrapWithSingleQuotes();
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff").WrapWithSingleQuotes();
            }

            return value;

        }

        private static string CustomStringFormatValue(PropertyInfo prop, object obj) 
        {
            return null;
        }
    }
}
