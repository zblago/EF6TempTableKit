using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Model;
using System;
using System.Collections.Generic;
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
                var sql = expression.ToTraceQuery();

                var objectQuery = expression.GetObjectQuery();

                var positions = objectQuery.GetQueryPropertyPositions();

                var tempTableType = expression.ElementType.FullName;
                if (dbContextExtended.TempSqlQueriesList.ContainsKey(tempTableType))
                {
                    throw new Exception("temp table key alred there");
                }

                dbContextExtended.InsertTempExpressions(tempTableType, sql);
                 
                return dbContextExtended as T;
            }
        }
    }
}
  