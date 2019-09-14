using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EF6TempTableKit.Extensions
{
    public static class IQueryableExtensions
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
            foreach (var parameter in objectQuery.Parameters)
            {
                if (parameter.Value == null)
                    continue;
                var name = "@" + parameter.Name;
                var value = "'" + parameter.Value.ToString() + "'";
                result = result.Replace(name, value);
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
    }
}
