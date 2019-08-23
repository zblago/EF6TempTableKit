using System.Data.Common;
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
            var dbContextInterceptor = interceptionContext.DbContexts.SingleOrDefault(db => db.GetType().BaseType == typeof(DbContextInterceptor));

            if (dbContextInterceptor != null && ((DbContextInterceptor)dbContextInterceptor).TempSqlQueriesList.Count > 0)
            {
                var currentCommandText = command.CommandText;
                foreach (var sqlTempQuery in ((DbContextInterceptor)dbContextInterceptor).TempSqlQueriesList)
                {
                    //apply sql utitliy here
                    //take field types from custom attribute
                    currentCommandText = sqlTempQuery + currentCommandText;
                }
                command.CommandText = currentCommandText;
            }
        }
    }
}