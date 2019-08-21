using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Model;
using System;
using System.Linq;

namespace SharDev.EFInterceptor.Extensions
{
    namespace SharkDev.Extensions
    {
        public static class DbContextExtensions
        {
            public static T WithCustomQuery<T>(this DbContextInterceptor dbContextExtended, Func<DbContextInterceptor, string, string> method) 
                where T : class
            {
                dbContextExtended.Method = method;

                return dbContextExtended as T;
            }

            public static T WithTempExpression<T>(this DbContextInterceptor dbContextExtended, IQueryable<ITempTable> expression)
                where T : class
            {
                //Add validation logic: 1. does expression per item already exist, 2. does method is typeof IQueryable

                dbContextExtended.InsertTempExpressions(expression);

                return dbContextExtended as T;
            }
        }
    }
}
  