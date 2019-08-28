using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Model;
using SharDev.EFInterceptor.SqlUtility;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

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

                var tableMetadataProvider = new TableMetadataProvider();

                var tempTableName = tableMetadataProvider.GetTableNameFromBaseType(expression.ElementType.BaseType);
                var tempTableCreator = new TempTableCreator(tempTableName);
                var fieldsAndTypes = tableMetadataProvider.GetFieldWithPositionsFromBaseType(expression.ElementType.BaseType);
                var ddlQuery = tempTableCreator.DropIfExists().Create(fieldsAndTypes).GetQuery();

                var tempTableExpressionsql = expression.ToTraceQuery();
                var insertQueryCreator = new InsertQueryCreator(tempTableName);
                var objectQuery = expression.GetObjectQuery();
                var positions = objectQuery.GetQueryPropertyPositions();
                var dmlQuery = insertQueryCreator.AddInsertQuery(positions, tempTableExpressionsql).GetQuery();

                var ddlWithDmlQuery = ddlQuery + dmlQuery;

                dbContextExtended.InsertTempExpressions(tempTableType, ddlWithDmlQuery);
                 
                return dbContextExtended as T;
            }
        }
    }
}
  