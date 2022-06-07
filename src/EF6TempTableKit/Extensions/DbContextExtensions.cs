﻿using System.Linq;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;
using EF6TempTableKit.Utilities;
using System.Collections.Generic;
using EF6TempTableKit.Exceptions;
using System;

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
        public static T WithTempTableExpression<T>(this T dbContexWithTempTable, IQueryable<ITempTable> expression)
            where T : class, IDbContextWithTempTable
        {
            var tableMetadataProvider = new TableMetadataProvider();
            var tempTableType = expression.ElementType.BaseType;
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(tempTableType);
            var hasAttachedDDLStatement = dbContexWithTempTable.TempTableContainer.TempSqlQueriesList.Any(x => x.Key == tempTableName);

            Validate(dbContexWithTempTable, tempTableName);

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

            var tempTableDependencyManager = new TempTableDependencyManager(objectQuery, dbContexWithTempTable.TempTableContainer);
            tempTableDependencyManager.AddDependenciesForTable(tempTableName);

            dbContexWithTempTable.TempTableContainer
                .TempSqlQueriesList
                .Enqueue(new KeyValuePair<string, Query>(tempTableName, new Query
                {
                    QueryString = sqlAllCommandsQuery,
                    IsDataAppend = hasAttachedDDLStatement
                }));

            return dbContexWithTempTable;

        }

        /// <summary>
        /// Use it to attach LINQ query being used to load data into temporary table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContexWithTempTable"></param>
        /// <param name="expression"></param>
        /// <param name="reuseExisting">Obsolete, don't use it</param>
        /// <returns></returns>
        [Obsolete("Use WithTempTableExpression(dbContexWithTempTable, expression)")]
        public static T WithTempTableExpression<T>(this T dbContexWithTempTable, IQueryable<ITempTable> expression, bool reuseExisting = false)
            where T : class, IDbContextWithTempTable
        {
            return dbContexWithTempTable.WithTempTableExpression(expression);
        }

        /// <summary>
        /// Use it to attach LINQ query built upon memory data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContexWithTempTable"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T WithTempTableExpression<T>(this T dbContexWithTempTable, IEnumerable<ITempTable> list)
            where T : class, IDbContextWithTempTable
        {
            var tableMetadataProvider = new TableMetadataProvider();
            var tempTableType = list.First().GetType();
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(tempTableType);
            var hasAttachedDDLStatement = dbContexWithTempTable.TempTableContainer.TempSqlQueriesList.Any(x => x.Key == tempTableName);

            Validate(dbContexWithTempTable, tempTableName);

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

            dbContexWithTempTable.TempTableContainer
                .TempSqlQueriesList
                .Enqueue(new KeyValuePair<string, Query>(tempTableName, new Query
                {
                    QueryString = sqlAllCommandsQuery,
                    IsDataAppend = hasAttachedDDLStatement
                }));

            return dbContexWithTempTable;

        }


        /// <summary>
        /// Use it to attach LINQ query built upon memory data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContexWithTempTable"></param>
        /// <param name="list"></param>
        /// <param name="reuseExisting">Obsolete, don't use it</param>
        /// <returns></returns>
        [Obsolete("Use WithTempTableExpression(dbContexWithTempTable, list)")]
        public static T WithTempTableExpression<T>(this T dbContexWithTempTable, IEnumerable<ITempTable> list, bool reuseExisting = false)
            where T : class, IDbContextWithTempTable
        {
            return dbContexWithTempTable.WithTempTableExpression(list);
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
        public static void ReinitializeTempTableContainer(this IDbContextWithTempTable dbContexWithTempTable)
        {
            dbContexWithTempTable.TempTableContainer.Reinitialize();
        }
    }
}
