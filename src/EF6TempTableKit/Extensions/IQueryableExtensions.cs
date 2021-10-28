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
        /// <param name="replaceParamsWithValues">True for backwards compatibility</param>
        /// <returns></returns>
        public static string ToTraceQuery<T>(this IQueryable<T> query, bool replaceParamsWithValues = true)
        {
            ObjectQuery objectQuery;
            var method = typeof(IQueryableExtensions).GetMethod("GetQueryFromQueryable");
            var genMethod = method.MakeGenericMethod(query.GetType().GenericTypeArguments[0]);
            objectQuery = (dynamic)genMethod.Invoke(null, new object[] { query });
            var result = objectQuery.ToTraceString();

            if (!replaceParamsWithValues) {
                return result;
            }

            List<Type> numericTypes = new List<Type>() { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal), typeof(double) };


            var paramsReversed = objectQuery.Parameters.Reverse();//reverse params order to avoid replacement of @p__linq__1 (with let's say value of '1') in @p__linq__11 as '1'1
            foreach (var parameter in paramsReversed)
            {
                object value = "";
                if (parameter.Value == null)
                    continue;
                var name = "@" + parameter.Name;

                if (replaceParamsWithValues)
                {
                    if (parameter.ParameterType == typeof(bool))
                    {
                        value = GetBoolStringValue(parameter.Value);
                    }
                    else if (!numericTypes.Contains(parameter.ParameterType)
                            && parameter.Value == null
                            || (parameter.ParameterType.IsValueType && Nullable.GetUnderlyingType(parameter.ParameterType) != null)
                            )
                    {
                        if (parameter.ParameterType == typeof(DateTime?))
                        {
                            value = parameter.Value != null ? ReplaceWithQuotes(parameter.Value) : "NULL";
                        }
                        else if (parameter.ParameterType == typeof(bool?))
                        {
                            value = parameter.Value != null ? GetBoolStringValue(parameter.Value) : "NULL";
                        }
                        else
                        {
                            value = parameter.Value != null ? parameter.Value.ToString() : "NULL";
                        }
                    }
                    else if (numericTypes.Contains(parameter.ParameterType))
                    {
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
    }
}
