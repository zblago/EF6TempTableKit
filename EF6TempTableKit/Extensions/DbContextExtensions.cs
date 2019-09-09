using System;
using System.Linq;
using EF6TempTableKit.Model;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;

namespace EF6TempTableKit.Extensions
{
    public static class DbContextExtensions
    {
        public static T WithCustomQuery<T>(this System.Data.Entity.DbContext dbContexWithTempTable, 
            Func<System.Data.Entity.DbContext, string, string> method) 
            where T : class
        {
            ((IDbContextWithTempTable)dbContexWithTempTable).TempTableContainer.Method = method;

            return dbContexWithTempTable as T;
        }

        public static T WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression)
            where T : class
        {
            var tempTableType = expression.ElementType.FullName;
            var contextWithTempTable = (IDbContextWithTempTable)dbContexWithTempTable;
            if (contextWithTempTable.TempTableContainer.TempSqlQueriesList.ContainsKey(tempTableType))
            {
                throw new Exception($"Can't override query for temp table {tempTableType} as it is already attached to the context.");
            }

            var tableMetadataProvider = new TableMetadataProvider();
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(expression.ElementType.BaseType);
            var fieldsWithTypes = tableMetadataProvider.GetFieldsWithTypes(expression.ElementType.BaseType);

            var sqlSelectQuery = expression.ToTraceQuery();
            var objectQuery = expression.GetObjectQuery();
            var fieldsWithPositions = objectQuery.GetQueryPropertyPositions();

            var sqlAllCommandsQuery = SqlInsertCommandBuilder
                .Begin(tempTableName)
                .DropIfExists()
                .Create(fieldsWithTypes)
                .AddInsertQuery(fieldsWithPositions, sqlSelectQuery)
                .Execute();

            contextWithTempTable.TempTableContainer.TempSqlQueriesList.Add(tempTableType, sqlAllCommandsQuery);
                 
            return dbContexWithTempTable as T;
        }
    }
}
  