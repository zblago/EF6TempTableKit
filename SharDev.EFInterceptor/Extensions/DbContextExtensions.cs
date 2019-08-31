using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Model;
using System;
using System.Linq;
using SharDev.EFInterceptor.SqlCommands;
using System.Collections.Generic;

namespace SharDev.EFInterceptor.Extensions
{
    interface IWithCustomQuery
    {
        T WithCustomQuery<T>() where T : class;
    } 

    public class TempTableHolder : IWithCustomQuery
    {
        private IDictionary<string, string> TempSqlQueriesList { set; get; }

        public static TempTableHolder Create(System.Data.Entity.DbContext dbContext) => new TempTableHolder(dbContext);

        public System.Data.Entity.DbContext _dbContext;

        public TempTableHolder(System.Data.Entity.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T WithCustomQuery<T>() where T : class
        {
            return _dbContext as T;
        }
    }

    namespace SharkDev.Extensions
    {
        public static class DbContextExtensions
        {
            public static TempTableHolder UseTempTablesOnContext(this System.Data.Entity.DbContext dbContext, string test)
            {
                return TempTableHolder.Create(dbContext);
            }

            public static T WithCustomQuery<T>(this DbContextInterceptor dbContextExtended, Func<DbContextInterceptor, string, string> method) 
                where T : class
            {
                dbContextExtended.Method = method;

                return dbContextExtended as T;
            }

            public static T WithTempExpression<T>(this DbContextInterceptor dbContextExtended, IQueryable<ITempTable> expression)
                where T : class
            {
                var tempTableType = expression.ElementType.FullName;
                if (dbContextExtended.TempSqlQueriesList.ContainsKey(tempTableType))
                {
                    throw new Exception("temp table key already there");
                }

                var tableMetadataProvider = new TableMetadataProvider();
                var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(expression.ElementType.BaseType);
                var fieldsWithTypes = tableMetadataProvider.GetFieldWithPositionsFromBaseType(expression.ElementType.BaseType);

                var sqlSelectQuery = expression.ToTraceQuery();
                var objectQuery = expression.GetObjectQuery();
                var fieldsWithPositions = objectQuery.GetQueryPropertyPositions();

                var sqlAllCommandsQuery = SqlInsertCommandBuilder
                    .Begin(tempTableName)
                    .DropIfExists()
                    .Create(fieldsWithTypes)
                    .AddInsertQuery(fieldsWithPositions, sqlSelectQuery)
                    .Execute();

                dbContextExtended.InsertTempExpressions(tempTableType, sqlAllCommandsQuery);
                 
                return dbContextExtended as T;
            }
        }
    }
}
  