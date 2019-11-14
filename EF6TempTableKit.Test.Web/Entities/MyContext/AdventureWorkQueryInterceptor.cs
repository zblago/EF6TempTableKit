using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace EF6TempTableKit.Test.Interceptors
{
    public class AdventureWorkQueryInterceptor : DbCommandInterceptor
    {
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            command.CommandText = "-- Just an additional interceptor \n" + command.CommandText;
        }
    }
}