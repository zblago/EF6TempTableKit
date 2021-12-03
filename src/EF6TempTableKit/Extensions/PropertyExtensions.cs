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
        public static object GetSqlValue(this PropertyInfo prop, object obj, IDictionary<string, Attribute[]> customConverter)
        {
            var hasStringCustomConverter = customConverter[prop.Name]?.Length > 0 && customConverter[prop.Name].ToList().Any(x => x.GetType() == typeof(StringConvertAttribute));
            var hasFuncCustomConverter = customConverter[prop.Name]?.Length > 0 && customConverter[prop.Name].ToList().Any(x => x.GetType() == typeof(CustomConverterAttribute));

            if (hasStringCustomConverter && hasFuncCustomConverter)
                throw new EF6TempTableKitGenericException("EF6TempTableKit: Field can't have associated both StringFormatAttribute and CustomConverterAttribute custom format attributes.");

            var value = prop.GetValue(obj, null);

            if (hasStringCustomConverter)
            {
                var format = ((StringConvertAttribute)customConverter[prop.Name].First()).Format;
                var formatProvider = ((StringConvertAttribute)customConverter[prop.Name].First()).FormatProvider;

                return formatProvider == null 
                    ? CustomStringFormatValue(value, format) 
                    : CustomStringFormatValue(formatProvider, value, format);
            }
            else if (hasFuncCustomConverter)
            {
                Type storeType = ((CustomConverterAttribute)customConverter[prop.Name].First()).Type;
                var instance = Activator.CreateInstance(storeType);
                PropertyInfo info  = instance.GetType().GetProperty(nameof(ICustomConverter<object, object>.Converter));
                object yourField = info.GetValue(instance);
                MethodInfo method = yourField.GetType().GetMethod(nameof(MethodBase.Invoke));

                return method.Invoke(yourField, new object []{ value });
            }
            else
            {
                return DefaultFormatValue(value, prop);
            }
        }

        public static object GetSqlValue(this PropertyInfo prop, object obj, IDictionary<string, ConverterProperties[]> customConverter)
        {
            var hasStringCustomConverter = customConverter[prop.Name]?.Length > 0 && customConverter[prop.Name].ToList().Any(x => x.StringConvertAttribute != null);
            var hasFuncCustomConverter = customConverter[prop.Name]?.Length > 0 && customConverter[prop.Name].ToList().Any(x => x.MethodInfo != null && x.Field != null);

            if (hasStringCustomConverter && hasFuncCustomConverter)
                throw new EF6TempTableKitGenericException("EF6TempTableKit: Field can't have associated both StringFormatAttribute and CustomConverterAttribute custom format attributes.");

            var value = prop.GetValue(obj, null);

            if (hasStringCustomConverter)
            {
                var format = customConverter[prop.Name].First().StringConvertAttribute.Format;
                var formatProvider = customConverter[prop.Name].First().StringConvertAttribute.FormatProvider;

                return formatProvider == null
                    ? CustomStringFormatValue(value, format)
                    : CustomStringFormatValue(formatProvider, value, format);
            }
            else if (hasFuncCustomConverter)
            {
                var funcDetails = customConverter[prop.Name].First();

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
