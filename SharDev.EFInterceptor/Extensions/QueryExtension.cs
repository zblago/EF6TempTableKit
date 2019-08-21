using System.Linq;

namespace SharDev.EFInterceptor.Extensions
{
    public static class QueryExtension
    {
        public static IQueryable<T> ExtendWithTempQuery<T>(this IQueryable<T> query)
        {
            return query;
        }
    }
}
