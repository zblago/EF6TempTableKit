using SharDev.EFInterceptor.Extensions;
using SharDev.EFInterceptor.Model;
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
            //var modifiedCommand = (interceptionContext.DbContexts.First() as DbContextInterceptor)
            //        .Method(interceptionContext.DbContexts.First() as DbContextInterceptor, command.CommandText);

            var dbContextInterceptor = interceptionContext.DbContexts.SingleOrDefault(db => db.GetType().BaseType == typeof(DbContextInterceptor));

            if (dbContextInterceptor != null && ((DbContextInterceptor)dbContextInterceptor).TempExpressionsList.Count > 0)
            {
                var currentCommandText = command.CommandText;
                foreach (var expression in ((DbContextInterceptor)dbContextInterceptor).TempExpressionsList)
                {
                    var sql = expression.ToTraceQuery<ITempTable>();

                    //apply sql utitliy here
                    currentCommandText = sql + currentCommandText;
                }
                command.CommandText = currentCommandText;
            }
        }
    }
}