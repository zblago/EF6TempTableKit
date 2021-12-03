using EF6TempTableKit.Attributes;
using EF6TempTableKit.Exceptions;
using EF6TempTableKit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EF6TempTableKit.Extensions
{
    internal static class PropertyExtensions
    {
        public static object GetSqlValue(this PropertyInfo prop, object obj, IDictionary<string, Attribute[]> customFormatter)
        {
            var hasStringCustomFormatter = customFormatter[prop.Name]?.Length > 0 && customFormatter[prop.Name].ToList().Any(x => x.GetType() == typeof(StringFormatAttribute));
            var hasFuncCustomFormatter = customFormatter[prop.Name]?.Length > 0 && customFormatter[prop.Name].ToList().Any(x => x.GetType() == typeof(CustomConverterAttribute));

            if (hasStringCustomFormatter && hasFuncCustomFormatter)
                throw new EF6TempTableKitGenericException("EF6TempTableKit: Field can't have associated both StringFormatAttribute and CustomConverterAttribute custom format attributes.");

            var value = prop.GetValue(obj, null);

            if (hasStringCustomFormatter)
            {
                var format = ((StringFormatAttribute)customFormatter[prop.Name].First()).Format;
                var formatProvider = ((StringFormatAttribute)customFormatter[prop.Name].First()).FormatProvider;

                return formatProvider == null 
                    ? CustomStringFormatValue(value, format) 
                    : CustomStringFormatValue(formatProvider, value, format);
            }
            else if (hasFuncCustomFormatter)
            {
                Type storeType = ((CustomConverterAttribute)customFormatter[prop.Name].First()).Type;
                var instance = Activator.CreateInstance(storeType);
                PropertyInfo info  = instance.GetType().GetProperty(nameof(ICustomFuncFormatter<object, object>.Formatter));
                object yourField = info.GetValue(instance);
                MethodInfo method = yourField.GetType().GetMethod(nameof(MethodBase.Invoke));

                return method.Invoke(yourField, new object []{ value });
            }
            else
            {
                return DefaultFormatValue(value, prop);
            }
        }

        public static object GetSqlValue(this PropertyInfo prop, object obj, IDictionary<string, FormatterProperties[]> customFormatter)
        {
            var hasStringCustomFormatter = customFormatter[prop.Name]?.Length > 0 && customFormatter[prop.Name].ToList().Any(x => x.StringFormatAttribute != null);
            var hasFuncCustomFormatter = customFormatter[prop.Name]?.Length > 0 && customFormatter[prop.Name].ToList().Any(x => x.MethodInfo != null && x.Field != null);

            if (hasStringCustomFormatter && hasFuncCustomFormatter)
                throw new EF6TempTableKitGenericException("EF6TempTableKit: Field can't have associated both StringFormatAttribute and CustomConverterAttribute custom format attributes.");

            var value = prop.GetValue(obj, null);

            if (hasStringCustomFormatter)
            {
                var format = customFormatter[prop.Name].First().StringFormatAttribute.Format;
                var formatProvider = customFormatter[prop.Name].First().StringFormatAttribute.FormatProvider;

                return formatProvider == null
                    ? CustomStringFormatValue(value, format)
                    : CustomStringFormatValue(formatProvider, value, format);
            }
            else if (hasFuncCustomFormatter)
            {
                var funcDetails = customFormatter[prop.Name].First();

                return funcDetails.MethodInfo.Invoke(funcDetails.Field, new object[] { value });
            }
            else
            {
                return DefaultFormatValue(value, prop);
            }
        }

        private static object DefaultFormatValue(object value, PropertyInfo prop) 
        {
            if (prop.PropertyType == typeof(bool))
            {
                value = (bool)value ? 1 : 0;
            }
            else if (prop.PropertyType == typeof(string))
            {
                value = value.ToString().WrapWithSingleQuotes();
            }
            else if (prop.PropertyType == typeof(DateTime))
            {
                value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.fff").WrapWithSingleQuotes();
            }

            return value;
        }

        private static string CustomStringFormatValue(object value, string format)
        {
            return string.Format(format, value);
        }

        private static string CustomStringFormatValue(IFormatProvider provider, object value, string format)
        {
            return string.Format(provider, format, value);
        }
    }
}
