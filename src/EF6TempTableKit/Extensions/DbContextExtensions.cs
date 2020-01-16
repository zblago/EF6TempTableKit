using System;
using System.Collections.Generic;
using System.Linq;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;

namespace EF6TempTableKit.Extensions
{
    public static class DbContextExtensions
    {
        public static T WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression, bool reuseExisting = false)
            where T : class
        {
            var tempTableTypeName = expression.ElementType.FullName;
            var contextWithTempTable = (IDbContextWithTempTable)dbContexWithTempTable;

            Validate(contextWithTempTable, tempTableTypeName);

            var tableMetadataProvider = new TableMetadataProvider();
            var tempTableType = expression.ElementType.BaseType;
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(tempTableType);
            var fieldsWithTypes = tableMetadataProvider.GetFieldsWithTypes(tempTableType);
            var fieldsForClusteredIndex = tableMetadataProvider.GetClusteredIndexColumns(tempTableType);
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
                    .AddClusteredIndex(fieldsForClusteredIndex)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQuery(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }
            else
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .CreateIfNotExists(fieldsWithTypes)
                    .AddClusteredIndex(fieldsForClusteredIndex)
                    .AddNonClusteredIndexes(nonClusteredIndexesWithFields)
                    .AddInsertQueryIfCreated(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }

            var existingTables = contextWithTempTable.TempTableContainer.TempSqlQueriesList.Keys;
            foreach (var table in existingTables)
            {
                var doesContain = sqlSelectQuery.Contains(table.ToString());
                if (doesContain)
                {
                    if (!contextWithTempTable.TempTableContainer.TempOnTempDependecies.ContainsKey(table.ToString()))
                    {
                        contextWithTempTable.TempTableContainer.TempOnTempDependecies = new Dictionary<string, string[]>();
                        contextWithTempTable.TempTableContainer.TempOnTempDependecies
                            .Add(new KeyValuePair<string, string[]>(table.ToString(), new string[] { tempTableName }));
                    }
                    else
                    {
                        var values = contextWithTempTable.TempTableContainer.TempOnTempDependecies[table.ToString()];
                        Array.Resize(ref values, values.Length);
                        values[values.Length] = tempTableName;
                        contextWithTempTable.TempTableContainer.TempOnTempDependecies[table.ToString()] = values;
                    }
                }
            }

            contextWithTempTable.TempTableContainer
                .TempSqlQueriesList.Add(tempTableName, new Query { QueryString = sqlAllCommandsQuery, ReuseExisting = reuseExisting });
                 
            return dbContexWithTempTable as T;
        }

        private static void Validate(IDbContextWithTempTable contextWithTempTable, string tempTableType)
        {
            if (contextWithTempTable.TempTableContainer == null)
            {
                throw new Exception($"Object of type TempTableContainer is not instantiated. Please, make an instance in your DbContext.");
            }

            if (contextWithTempTable.TempTableContainer.TempSqlQueriesList[tempTableType] != null)
            {
                throw new Exception($"Can't override query for temp table {tempTableType} as it is already attached to the context.");
            }
        }
    }
}
  