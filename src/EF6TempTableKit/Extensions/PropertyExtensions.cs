using EF6TempTableKit.Exceptions;
using System;
using System.Reflection;

namespace EF6TempTableKit.Extensions
{
    internal static class PropertyExtensions
    {
        public static object GetSqlValue(this PropertyInfo prop, object obj)
        {
            var value = prop.GetValue(obj, null);

            var intType = typeof(int);

            if (prop.PropertyType == typeof(bool))
            {
            }
            else if (prop.PropertyType == typeof(byte))
            {
            }
            else if (prop.PropertyType == typeof(sbyte))
            {
            }
            else if (prop.PropertyType == typeof(char))
            {
            }
            else if (prop.PropertyType == typeof(decimal))
            {
            }
            else if (prop.PropertyType == typeof(double))
            {
            }
            else if (prop.PropertyType == typeof(float))
            {
            }
            else if (prop.PropertyType == typeof(int))
            {
            }
            else if (prop.PropertyType == typeof(uint))
            {
            }
            else if (prop.PropertyType == typeof(long))
            {
            }
            else if (prop.PropertyType == typeof(ulong))
            {
            }
            else if (prop.PropertyType == typeof(short))
            {
            }
            else if (prop.PropertyType == typeof(ushort))
            {
            }
            else if (prop.PropertyType == typeof(string))
            {
                value = value.SurroundWithSingleQuotes();
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
            }
            else
            {
                throw new EF6TempTableGenericException("EF6TempTableKit: Not supported data type.");
            }

            return value;
        }
    }
}
