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
            if ((interceptionContext.DbContexts.First() as DbContextInterceptor).Method != null)
            {
                var modifiedCommand = (interceptionContext.DbContexts.First() as DbContextInterceptor)
                    .Method(interceptionContext.DbContexts.First() as DbContextInterceptor, command.CommandText);

                command.CommandText = modifiedCommand;
            }
        }
    }
}