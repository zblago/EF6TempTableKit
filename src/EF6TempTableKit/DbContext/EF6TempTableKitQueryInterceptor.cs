using EF6TempTableKit.Utilities;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using EF6TempTableKit.Extensions;

namespace EF6TempTableKit.DbContext
{
    public sealed class EF6TempTableKitQueryInterceptor : DbCommandInterceptor
    {
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            PrependTempTableSql(command, interceptionContext);
        }

        private void PrependTempTableSql<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            var dbContextWithTempTable = FindDbContextWithTempTable(interceptionContext.DbContexts);
            var tableContainer = ((IDbContextWithTempTable)dbContextWithTempTable)?.TempTableContainer;
            if (tableContainer?.TempSqlQueriesList?.Count > 0)
            {
                var sqlFromTempTableDependenciesBuilder = new SqlFromTempTableDependenciesBuilder(((IDbContextWithTempTable)dbContextWithTempTable).TempTableContainer);
                command.CommandText = sqlFromTempTableDependenciesBuilder.BuildSqlForTempTables(command.CommandText) + command.CommandText;

                if (tableContainer.ReinitializeOnExecute)
                {
                    tableContainer.Reinitialize();
                }
            }
        }

        private IDbContextWithTempTable FindDbContextWithTempTable(IEnumerable<System.Data.Entity.DbContext> dbContexts)
        {
            foreach (var context in dbContexts)
            {
                if (context is IDbContextWithTempTable withTemp)
                {
                    return withTemp;
                }
            }
            return null;
        }
    }
}