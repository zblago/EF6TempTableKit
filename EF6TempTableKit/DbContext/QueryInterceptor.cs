using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace EF6TempTableKit.DbContext
{
    public sealed class QueryInterceptor : DbCommandInterceptor
    {
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            AddModifyMethod(command, interceptionContext);
        }

        private void AddModifyMethod<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            var dbContextWithTempTable = FindDbContextWithTempTable(interceptionContext.DbContexts);
            if (dbContextWithTempTable != null && ((IDbContextWithTempTable) dbContextWithTempTable).TempTableContainer.TempSqlQueriesList.Count > 0)
            {
                var currentCommandText = command.CommandText;
                var contextWithTempTable = (IDbContextWithTempTable)dbContextWithTempTable;
                foreach (var sqlTempQuery in contextWithTempTable.TempTableContainer.TempSqlQueriesList)
                {
                    currentCommandText = sqlTempQuery.Value + currentCommandText;
                }
                command.CommandText = currentCommandText;

                contextWithTempTable.TempTableContainer.TempSqlQueriesList.Clear();
            }
        }

        private System.Data.Entity.DbContext FindDbContextWithTempTable(IEnumerable<System.Data.Entity.DbContext> dbContexts)
        {
            foreach (var context in dbContexts)
            {
                if (context is IDbContextWithTempTable)
                {
                    return context;
                }
            }
            return null;
        }
    }
}