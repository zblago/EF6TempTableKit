using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace EFIntercept.Context
{
    public class AdventureWorkQueryInterceptor : DbCommandInterceptor
    {
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            var s = 1;
        }
    }
}