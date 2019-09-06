using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Model;
using System;
using System.Linq;
using SharDev.EFInterceptor.SqlCommands;
using System.Collections.ObjectModel;

namespace SharDev.EFInterceptor.Extensions
{
    namespace SharkDev.Extensions
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

                var internalQuerySelect = expression.GetType()
                    .GetProperty("InternalQuery", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(expression, null)
                        .GetType()
                        .GetProperty("Expression")
                        .GetValue(expression.GetType().GetProperty("InternalQuery", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(expression, null), null);

                var arguments = internalQuerySelect.GetType().GetProperty("Arguments").GetValue(internalQuerySelect, null);
                var argumentValues = (arguments as ReadOnlyCollection<System.Linq.Expressions.Expression>);

                var unaryExp = (expression.Expression as System.Linq.Expressions.MethodCallExpression).Arguments as ReadOnlyCollection<System.Linq.Expressions.Expression>;
                
                foreach (var ue in unaryExp)
                {
                    var t = ue;

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

                dbContextExtended.InsertTempExpressions(tempTableType, sqlAllCommandsQuery);
                 
                return dbContextExtended as T;
            }
        }
    }
}
  