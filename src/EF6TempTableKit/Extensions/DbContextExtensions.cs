using System.Linq;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;
using EF6TempTableKit.Utilities;
using System.Collections.Generic;
using EF6TempTableKit.Exceptions;

namespace EF6TempTableKit.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Use it to attach LINQ query being used to load data into temporary table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContexWithTempTable"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression)
            where T : class
        {
            var contextWithTempTable = (IDbContextWithTempTable)dbContexWithTempTable;
            var tableMetadataProvider = new TableMetadataProvider();
            var tempTableType = expression.ElementType.BaseType;
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(tempTableType);
            var hasAttachedDDLStatement = contextWithTempTable.TempTableContainer.TempSqlQueriesList.Any(x => x.Key == tempTableName);

            Validate(contextWithTempTable, tempTableName);

            var fieldsWithTypes = tableMetadataProvider.GetFieldsWithTypes(tempTableType);
            var clusteredIndexesWithFields = tableMetadataProvider.GetClusteredIndexColumns(tempTableType);
            var nonClusteredIndexesWithFields = tableMetadataProvider.GetNonClusteredIndexesWithColumns(tempTableType);

            var sqlSelectQuery = expression.ToTraceQuery();
            var objectQuery = expression.GetObjectQuery();
            var fieldsWithPositions = objectQuery.GetQueryPropertyPositions();

            var sqlAllCommandsQuery = "";

            if (hasAttachedDDLStatement)
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Continue(tempTableName)
                    .AddInsertQuery(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }
            else
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .DropIfExists()
                    .Create(fieldsWithTypes)
                    .AddClusteredIndex(clusteredIndexesWithFields)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQuery(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }

            var tempTableDependencyManager = new TempTableDependencyManager(objectQuery, contextWithTempTable.TempTableContainer);
            tempTableDependencyManager.AddDependenciesForTable(tempTableName);

            contextWithTempTable.TempTableContainer
                .TempSqlQueriesList
                .Enqueue(new KeyValuePair<string, Query>(tempTableName, new Query
                {
                    QueryString = sqlAllCommandsQuery,
                    IsDataAppend = hasAttachedDDLStatement
                }));

            return dbContexWithTempTable as T;
        }

        /// <summary>
        /// Use it to attach LINQ query built upon memory data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContexWithTempTable"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IEnumerable<ITempTable> list)
            where T : class
        {
            var tableMetadataProvider = new TableMetadataProvider();
            var contextWithTempTable = (IDbContextWithTempTable)dbContexWithTempTable;
            var tempTableType = list.First().GetType();
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(tempTableType);
            var hasAttachedDDLStatement = contextWithTempTable.TempTableContainer.TempSqlQueriesList.Any(x => x.Key == tempTableName);

            Validate(contextWithTempTable, tempTableName);

            var fieldsWithTypes = tableMetadataProvider.GetFieldsWithTypes(tempTableType);
            var clusteredIndexesWithFields = tableMetadataProvider.GetClusteredIndexColumns(tempTableType);
            var nonClusteredIndexesWithFields = tableMetadataProvider.GetNonClusteredIndexesWithColumns(tempTableType);

            var sqlAllCommandsQuery = "";

            if (hasAttachedDDLStatement)
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Continue(tempTableName)
                    .AddInsertQuery(list)
                    .Execute();
            }
            else
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .DropIfExists()
                    .Create(fieldsWithTypes)
                    .AddClusteredIndex(clusteredIndexesWithFields)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQuery(list)
                    .Execute();
            }

            contextWithTempTable.TempTableContainer
                .TempSqlQueriesList
                .Enqueue(new KeyValuePair<string, Query>(tempTableName, new Query
                {
                    QueryString = sqlAllCommandsQuery,
                    IsDataAppend = hasAttachedDDLStatement
                }));

            return dbContexWithTempTable as T;
        }

        private static void Validate(IDbContextWithTempTable contextWithTempTable, string tempTableName)
        {
            if (contextWithTempTable.TempTableContainer == null)
            {
                throw new EF6TempTableKitGenericException($"EF6TempTableKit: Object of type TempTableContainer is not instantiated. Please, make an instance in your DbContext.");
            }
        }

        /// <summary>
        /// Reinitializes internal storage - TempTableContainer, so you can attach again LINQ query being used to load data into temporary table.
        /// </summary>
        /// <param name="dbContexWithTempTable"></param>
        public static void ReinitializeTempTableContainer(this System.Data.Entity.DbContext dbContexWithTempTable)
        {
            ((IDbContextWithTempTable)dbContexWithTempTable).TempTableContainer = new TempTableContainer();
        }
    }
}
  