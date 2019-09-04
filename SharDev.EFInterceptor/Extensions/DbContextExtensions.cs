using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Model;
using System;
using System.Linq;
using SharDev.EFInterceptor.SqlCommands;

namespace SharDev.EFInterceptor.Extensions
{ 
    public static class DbContextExtensions
    {
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
  