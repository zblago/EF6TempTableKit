using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;


namespace EFIntercept.Context
{
    public class QueryInterceptor : DbCommandInterceptor
    {
        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            AddLockStatement(command, interceptionContext);
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            AddLockStatement(command, interceptionContext);
        }

        private void AddLockStatement<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            if ((interceptionContext.DbContexts.First() as AdventureWorksDW2008R2EntitiesCustomized).Method != null)
            {
                var modifiedCommand = (interceptionContext.DbContexts.First() as AdventureWorksDW2008R2EntitiesCustomized)
                    .Method(interceptionContext.DbContexts.First() as AdventureWorksDW2008R2EntitiesCustomized, command.CommandText);

                command.CommandText = modifiedCommand;
            }

            //var lockMode = GetLock(interceptionContext);

            //switch (lockMode)
            //{
            //    case SqlLockMode.UpdLock:
            //        command.CommandText = SqlModifier.AddUpdLock(command.CommandText);
            //        break;
            //}
        }

        //private SqlLockMode GetLock<T>(DbCommandInterceptionContext<T> interceptionContext)
        //{
        //    if (interceptionContext == null) return SqlLockMode.None;

        //    ILockContext lockContext = interceptionContext.DbContexts.First() as ILockContext;

        //    if (lockContext == null) return SqlLockMode.None;

        //    return lockContext.LockMode;
        //}

        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //(interceptionContext.DbContexts.First() as AdventureWorksDW2008R2EntitiesCustomized).Method = null;
        }
    }
}