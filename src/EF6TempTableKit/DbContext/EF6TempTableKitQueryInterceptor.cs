using System;
using EF6TempTableKit.Utilities;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Linq;
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
            var tableContainer = dbContextWithTempTable?.TempTableContainer;
            if (tableContainer?.TempSqlQueriesList?.Count > 0)
            {
                var sqlFromTempTableDependenciesBuilder = new SqlFromTempTableDependenciesBuilder(dbContextWithTempTable.TempTableContainer);
                var tempTableSql = sqlFromTempTableDependenciesBuilder.BuildSqlForTempTables(command.CommandText, out var additionalParameters);
                command.CommandText = tempTableSql + command.CommandText;
                foreach (var p in additionalParameters)
                {
                    var foo = command.CreateParameter();
                    command.Parameters.Add(new SqlParameter(p.Name, p.Value ?? DBNull.Value));
                }

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