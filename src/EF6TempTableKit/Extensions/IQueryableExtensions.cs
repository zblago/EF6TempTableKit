using System.Data.Entity.Core.Objects;
using System.Linq;

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

            objectQuery = (dynamic)genMethod.Invoke(null, new object[] { query });

            var result = objectQuery.ToTraceString();

            var paramsReversed = objectQuery.Parameters.Reverse();//reverse params order to avoid replacement of @p__linq__1 with, let's say value 'Joe' in @p__linq__11 as 'Joe'1 in result variable
            foreach (var parameter in paramsReversed)
            {
                if (parameter.Value == null)
                    continue;
                var name = "@" + parameter.Name;
                if (parameter.ParameterType == typeof(bool))
                {
                    var value = parameter.Value.ToString().ToLower() == "false" ? 0 : 1;
                    result = result.Replace(name, value.ToString());
                }
                else
                {
                    var value = "'" + parameter.Value.ToString() + "'";
                    result = result.Replace(name, value);
                }
            }
            return result;

        }
        public static ObjectQuery<T> GetQueryFromQueryable<T>(IQueryable<T> query)
        {
            var internalQueryField = query.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(f => f.Name.Equals("_internalQuery")).FirstOrDefault();
            var internalQuery = new object();

            //If query is wrapped with LinqKit extensions we have to get InnerQuery and InternalQuery from it afterwards.
            if (internalQueryField == null)
            {
                var innerQuery = query.GetType()
                    .GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .FirstOrDefault(f => f.Name.Equals("InnerQuery"))
                    .GetValue(query);
                internalQuery = innerQuery.GetType()
                    .GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .FirstOrDefault(f => f.Name.Equals("InternalQuery"))
                    .GetValue(innerQuery);
            }
            else
            {
                internalQuery = internalQueryField.GetValue(query);
            }

            var objectQueryField = internalQuery.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(f => f.Name.Equals("_objectQuery")).FirstOrDefault();
            var objectQueryValue = objectQueryField.GetValue(internalQuery);

            return (dynamic)objectQueryValue;
        }
    }
}
