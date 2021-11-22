﻿
using System.Linq;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;
using EF6TempTableKit.Utilities;
using System.Collections.Generic;
using EF6TempTableKit.Exceptions;
using EF6TempTableKit.Interfaces;

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
        /// <param name="reuseExisting"></param>
        /// <returns></returns>
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
            tempTableDependencyManager.AddDependenciesForTable(tempTableName);

            contextWithTempTable.TempTableContainer
                .TempSqlQueriesList
                .Enqueue(new KeyValuePair<string, Query>(tempTableName, new Query { QueryString = sqlAllCommandsQuery, ReuseExisting = reuseExisting, QueryType = Enums.QueryType.DB }));

            return dbContexWithTempTable as T;
        }

        /// <summary>
        /// Use it to attach LINQ query built upon memory data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContexWithTempTable"></param>
        /// <param name="list"></param>
        /// <param name="reuseExisting"></param>
        /// <returns></returns>
        public static T WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IEnumerable<ITempTable> list, bool reuseExisting = false)
            where T : class
        {
            var tableMetadataProvider = new TableMetadataProvider();
            var contextWithTempTable = (IDbContextWithTempTable)dbContexWithTempTable;
            var tempTableType = list.First().GetType();
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(tempTableType);

            Validate(contextWithTempTable, tempTableName);

            var fieldsWithTypes = tableMetadataProvider.GetFieldsWithTypes(tempTableType);
            var clusteredIndexesWithFields = tableMetadataProvider.GetClusteredIndexColumns(tempTableType);
            var nonClusteredIndexesWithFields = tableMetadataProvider.GetNonClusteredIndexesWithColumns(tempTableType);

            var sqlAllCommandsQuery = "";
            if (!reuseExisting)
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .DropIfExists()
                    .Create(fieldsWithTypes)
                    .AddClusteredIndex(clusteredIndexesWithFields)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQuery(list)
                    .Execute();
            }
            else
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .CreateIfNotExists(fieldsWithTypes)
                    .AddClusteredIndex(clusteredIndexesWithFields)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQueryIfCreated(list)
                    .Execute();
            }

            contextWithTempTable.TempTableContainer
                .TempSqlQueriesList
                .Enqueue(new KeyValuePair<string, Query>(tempTableName, new Query { QueryString = sqlAllCommandsQuery, ReuseExisting = reuseExisting, QueryType = Enums.QueryType.InMemory }));

            return dbContexWithTempTable as T;
        }
        private static void Validate(IDbContextWithTempTable contextWithTempTable, string tempTableName)
        {
            if (contextWithTempTable.TempTableContainer == null)
            {
                throw new EF6TempTableKitGenericException($"EF6TempTableKit: Object of type TempTableContainer is not instantiated. Please, make an instance in your DbContext.");
            }

            if (contextWithTempTable.TempTableContainer.TempSqlQueriesList.GroupBy(x => x.Value.QueryType).Any(x => x.Count(i => i.Key == tempTableName) > 1))
            {
                throw new EF6TempTableKitGenericException($"EF6TempTableKit: Can't override query for temp table {tempTableName} as it is already attached to the context.");
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
  