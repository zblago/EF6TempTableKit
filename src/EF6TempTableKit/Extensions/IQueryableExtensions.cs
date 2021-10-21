using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text.RegularExpressions;

namespace EF6TempTableKit.Extensions
{
    internal static class IQueryableExtensions
    {
        /// <summary>
        /// For an Entity Framework IQueryable, returns the SQL with inlined Parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string ToTraceQuery<T>(this IQueryable<T> query)
        {
            ObjectQuery objectQuery;
            var method = typeof(IQueryableExtensions).GetMethod("GetQueryFromQueryable");
            var genMethod = method.MakeGenericMethod(query.GetType().GenericTypeArguments[0]);
            List<Type> numericTypes = new List<Type>() { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal), typeof(double) };
            objectQuery = (dynamic)genMethod.Invoke(null, new object[] { query });

            var result = objectQuery.ToTraceString();

            var paramsReversed = objectQuery.Parameters.Reverse();//revers params order to avoid replacement of @__p__1 with e.i. '1' in @__p__11 as '1'1
            foreach (var parameter in paramsReversed)
            {
                object value = "";
                if (parameter.Value == null)
                    continue;
                var name = "@" + parameter.Name;

                if (parameter.ParameterType == typeof(bool))
                {
                    value = GetBoolStringValue(parameter.Value);
                }
                else if (!numericTypes.Contains(parameter.ParameterType) && IsNullableValueType(parameter.ParameterType))
                {
                    if (parameter.ParameterType == typeof(DateTime?))
                    {
                        value = parameter.Value != null ? ReplaceWithQuotes(parameter.Value) : "NULL";
                    }
                    else if (parameter.ParameterType == typeof(bool?))
                    {
                        value = parameter.Value != null ? GetBoolStringValue(parameter.Value) : "NULL";
                    }
                    else {
                        value = parameter.Value != null ? parameter.Value.ToString() : "NULL";
                    }
                }
                else if (numericTypes.Contains(parameter.ParameterType)) {
                    value = parameter.Value.ToString();
                }
                else if (parameter.ParameterType.IsEnum && numericTypes.Contains(Enum.GetUnderlyingType(parameter.ParameterType)))
                {
                    var underlyingType = Enum.GetUnderlyingType(parameter.ParameterType);
                    object underlyingValue = Convert.ChangeType(parameter.Value, underlyingType);
                    value = underlyingValue.ToString();
                }
               else 
                {
                    value = ReplaceWithQuotes(parameter.Value);
                }

                result = result.Replace(name, value.ToString());
            }
            return result;

        }
        public static ObjectQuery<T> GetQueryFromQueryable<T>(IQueryable<T> query)
        {
            var internalQueryField = query.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(f => f.Name.Equals("_internalQuery")).FirstOrDefault();
            var internalQuery = internalQueryField.GetValue(query);
            var objectQueryField = internalQuery.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(f => f.Name.Equals("_objectQuery")).FirstOrDefault();
            var objectQueryValue = objectQueryField.GetValue(internalQuery);

            return (dynamic)objectQueryValue;
        }

        static string ReplaceWithQuotes(object value) {
            if (value == null) {
                return "''";
            }

            return "'" + value.ToString() + "'";
        }

        static string GetBoolStringValue(object value) {
            if (value == null) {
                return "0";
            }
            return value.ToString().ToLower() == "false" ? "0" : "1";
        }

        static bool IsNullableValueType<T>(T obj)
        {
            //https://stackoverflow.com/questions/374651/how-to-check-if-an-object-is-nullable
            if (obj == null) return true;
            Type type = typeof(T);
            return type.IsValueType && Nullable.GetUnderlyingType(type) != null;
        }
    }
}
