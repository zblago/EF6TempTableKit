using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace SharDev.EFInterceptor.DbContext
{
    internal class QueryInterceptor : DbCommandInterceptor
    {
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            AddModifyMethod(command, interceptionContext);
        }

        private void AddModifyMethod<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            //var dbContextInterceptor = interceptionContext.DbContexts.SingleOrDefault(db => db.GetType().BaseType == typeof(DbContextInterceptor));

            DbContextInterceptor dbContextInterceptor = GetInterceptionContext(interceptionContext.DbContexts);
            if (dbContextInterceptor != null && (dbContextInterceptor).TempSqlQueriesList.Count > 0)
            {
                var currentCommandText = command.CommandText;
                foreach (var sqlTempQuery in (dbContextInterceptor).TempSqlQueriesList)
                {
                    currentCommandText = sqlTempQuery.Value + currentCommandText;
                }
                command.CommandText = currentCommandText;
            }
        }

        private DbContextInterceptor GetInterceptionContext(IEnumerable<System.Data.Entity.DbContext> dbContexts)
        {
            foreach (var context in dbContexts)
            {
                var type = GetDbContextInterceptorBaseType(context.GetType());
                if (type == typeof(DbContextInterceptor))
                {
                    return (DbContextInterceptor)context;
                }
            }
            return null;
        }

        private Type GetDbContextInterceptorBaseType(Type type)
        {
            if (type.BaseType == null)
                return type;

            if (type.BaseType == typeof(DbContextInterceptor))
            {
                return type.BaseType;
            }
            else
            {
                return GetDbContextInterceptorBaseType(type.BaseType);
            }
        }
    }
}