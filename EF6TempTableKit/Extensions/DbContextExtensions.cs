using System;
using System.Linq;
using EF6TempTableKit.Model;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;

namespace EF6TempTableKit.Extensions
{
    public static class DbContextExtensions
    {
        public static T WithCustomQuery<T>(this DbContextWithTempTable dbContexWithTempTable, Func<DbContextWithTempTable, string, string> method) 
            where T : class
        {
            dbContexWithTempTable.Method = method;

            return dbContexWithTempTable as T;
        }

        public static T WithTempTableExpression<T>(this DbContextWithTempTable dbContexWithTempTable, IQueryable<ITempTable> expression)
            where T : class
        {
            var tempTableType = expression.ElementType.FullName;
            if (dbContexWithTempTable.TempSqlQueriesList.ContainsKey(tempTableType))
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

            dbContexWithTempTable.InsertTempExpressions(tempTableType, sqlAllCommandsQuery);
                 
            return dbContexWithTempTable as T;
        }
    }
}
  