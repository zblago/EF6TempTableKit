using EF6TempTableKitNET8.Test.CodeFirst;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

public class RecordFinalQueryInterceptor : DbCommandInterceptor
{
    public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
        if (interceptionContext.DbContexts.FirstOrDefault() != null && interceptionContext.DbContexts.FirstOrDefault() is AdventureWorksCodeFirst adventureWorksCodeFirst)
        {
            adventureWorksCodeFirst.GeneratedTSQL = command.CommandText;
        }
    }
}