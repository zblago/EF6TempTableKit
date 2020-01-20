using System;
using System.Linq;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;
using EF6TempTableKit.Utilities;

namespace EF6TempTableKit.Extensions
{
    public static class DbContextExtensions
    {
        public static T WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression, bool reuseExisting = false)
            where T : class
        {
            var contextWithTempTable = (IDbContextWithTempTable)dbContexWithTempTable;
            var tableMetadataProvider = new TableMetadataProvider();
            var tempTableType = expression.ElementType.BaseType;
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(tempTableType);

            Validate(contextWithTempTable, tempTableName);

            var fieldsWithTypes = tableMetadataProvider.GetFieldsWithTypes(tempTableType);
            var clusteredIndexesWithFields = tableMetadataProvider.GetClusteredIndexColumns(tempTableType);
            var nonClusteredIndexesWithFields = tableMetadataProvider.GetNonClusteredIndexesWithColumns(tempTableType);

            var sqlSelectQuery = expression.ToTraceQuery();
            var objectQuery = expression.GetObjectQuery();
            var fieldsWithPositions = objectQuery.GetQueryPropertyPositions();

            var sqlAllCommandsQuery = "";

            if (!reuseExisting)
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .DropIfExists()
                    .Create(fieldsWithTypes)
                    .AddClusteredIndex(clusteredIndexesWithFields)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQuery(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }
            else
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .CreateIfNotExists(fieldsWithTypes)
                    .AddClusteredIndex(clusteredIndexesWithFields)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQueryIfCreated(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }

            var tempTableDependencyManager = new TempTableDependencyManager(sqlSelectQuery, objectQuery, contextWithTempTable.TempTableContainer);
            tempTableDependencyManager.AddDependencyTreeForTable(tempTableName);

            contextWithTempTable.TempTableContainer
                .TempSqlQueriesList
                .Add(tempTableName, new Query { QueryString = sqlAllCommandsQuery, ReuseExisting = reuseExisting });

            return dbContexWithTempTable as T;
        }

        private static void Validate(IDbContextWithTempTable contextWithTempTable, string tempTableName)
        {
            if (contextWithTempTable.TempTableContainer == null)
            {
                throw new Exception($"Object of type TempTableContainer is not instantiated. Please, make an instance in your DbContext.");
            }

            if (contextWithTempTable.TempTableContainer.TempSqlQueriesList[tempTableName] != null)
            {
                throw new Exception($"Can't override query for temp table {tempTableName} as it is already attached to the context.");
            }
        }
    }
}
  