using System;
using System.Linq;
using EF6TempTableKit.Model;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.SqlCommands;

namespace EF6TempTableKit.Extensions
{
    public static class DbContextExtensions
    {
        public static T WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression, bool reuseExisting = false)
            where T : class
        {
            var tempTableType = expression.ElementType.FullName;
            var contextWithTempTable = (IDbContextWithTempTable)dbContexWithTempTable;

            Validate(contextWithTempTable, tempTableType);

            var tableMetadataProvider = new TableMetadataProvider();
            var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(expression.ElementType.BaseType);
            var fieldsWithTypes = tableMetadataProvider.GetFieldsWithTypes(expression.ElementType.BaseType);

            var sqlSelectQuery = expression.ToTraceQuery();
            var objectQuery = expression.GetObjectQuery();
            var fieldsWithPositions = objectQuery.GetQueryPropertyPositions();

            var sqlAllCommandsQuery = ""; 

            if (!reuseExisting)
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .DropIfExists()
                    .Create(fieldsWithTypes)
                    .AddInsertQuery(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }
            else
            {
                sqlAllCommandsQuery = SqlInsertCommandBuilder.Begin(tempTableName)
                    .CreateIfNotExists(fieldsWithTypes)
                    .AddInsertQueryIfCreated(fieldsWithPositions, sqlSelectQuery)
                    .Execute();
            }

            contextWithTempTable.TempTableContainer.TempSqlQueriesList.Add(tempTableName, 
                new QueryString { Query = sqlAllCommandsQuery, ReuseExisting = reuseExisting });
                 
            return dbContexWithTempTable as T;
        }

        private static void Validate(IDbContextWithTempTable contextWithTempTable, string tempTableType)
        {
            if (contextWithTempTable.TempTableContainer == null)
            {
                throw new Exception($"Object of type TempTableContainer is not instantiated. Please, make an instance in your DbContext.");
            }

            if (contextWithTempTable.TempTableContainer.TempSqlQueriesList.ContainsKey(tempTableType))
            {
                throw new Exception($"Can't override query for temp table {tempTableType} as it is already attached to the context.");
            }
        }
    }
}
  