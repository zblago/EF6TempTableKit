using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace EF6TempTableKit.DbContext
{
    internal class QueryInterceptor : DbCommandInterceptor
    {
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            AddModifyMethod(command, interceptionContext);
        }

        private void AddModifyMethod<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            var dbContextWithTempTable = FindDbContextWithTempTable(interceptionContext.DbContexts);
            if (dbContextWithTempTable != null && dbContextWithTempTable.TempSqlQueriesList.Count > 0)
            {
                var currentCommandText = command.CommandText;
                foreach (var sqlTempQuery in dbContextWithTempTable.TempSqlQueriesList)
                {
                    currentCommandText = sqlTempQuery.Value + currentCommandText;
                }
                command.CommandText = currentCommandText;

                dbContextWithTempTable.TempSqlQueriesList.Clear();
            }
        }

        private DbContextWithTempTable FindDbContextWithTempTable(IEnumerable<System.Data.Entity.DbContext> dbContexts)
        {
            foreach (var context in dbContexts)
            {
                var type = GetDbContextWithTempTableBaseType(context.GetType());
                if (type == typeof(DbContextWithTempTable))
                {
                    return (DbContextWithTempTable)context;
                }
            }
            return null;
        }

        private Type GetDbContextWithTempTableBaseType(Type type)
        {
            if (type.BaseType == null)
                return type;

            if (type.BaseType == typeof(DbContextWithTempTable))
            {
                return type.BaseType;
            }
            else
            {
                return GetDbContextWithTempTableBaseType(type.BaseType);
            }
        }
    }
}