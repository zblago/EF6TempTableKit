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

                var tempTableExpressionsql = expression.ToTraceQuery();
                var objectQuery = expression.GetObjectQuery();
                var positions = objectQuery.GetQueryPropertyPositions();

                var tempTableNameFromConstructor = expression.ElementType.BaseType.CustomAttributes.Single(cs => cs.AttributeType == typeof(TableAttribute)).ConstructorArguments[0].Value;
                var tempTableNameFromArguments = expression.ElementType.BaseType.CustomAttributes.Single(cs => cs.AttributeType == typeof(TableAttribute)).NamedArguments.Where(n => n.MemberName == nameof(TableAttribute.Name));
                var tempTableName = tempTableNameFromConstructor != null ? tempTableNameFromConstructor.ToString() : tempTableNameFromArguments.ToString();

                var fields = expression.ElementType.BaseType.GetProperties().Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof (Attributes.TempFieldTypeAttribute)));
                var fieldsAndTypes = fields.Select(f => new
                {
                    f.Name,
                    Type = f.CustomAttributes.Single(ca => ca.AttributeType == typeof(Attributes.TempFieldTypeAttribute)).ConstructorArguments.Single().Value.ToString()
                }).ToDictionary(ft => ft.Name, ft => ft.Type);

                var tempTableCreator = new TempTableCreator(tempTableName);
                var ddlQuery = tempTableCreator.DropIfExists().Create(fieldsAndTypes).GetQuery();

                var insertQueryCreator = new InsertQueryCreator(tempTableName);
                var dmlQuery = insertQueryCreator.AddInsertQuery(positions, tempTableExpressionsql).GetQuery();

                var ddlWithDmlQuery = ddlQuery + dmlQuery;

                dbContextExtended.InsertTempExpressions(tempTableType, ddlWithDmlQuery);
                 
                return dbContextExtended as T;
            }
        }
    }
}
  