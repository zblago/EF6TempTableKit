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

                var fields = expression.ElementType.BaseType.GetProperties().Where(p => p.CustomAttributes.Any(ca => ca.AttributeType == typeof (Attributes.TempFieldTypeAttribute)));
                var fieldName = fields.Select(f => new
                    {
                        Name = f.Name,
                        FieldType = f.CustomAttributes.Single(ca => ca.AttributeType == typeof(Attributes.TempFieldTypeAttribute))
                            .ConstructorArguments.Single().Value
                });


                var tempTableCreator = new TempTableCreator();
                //tempTableCreator.CreateTempTable("").AddInsertQuery();

                dbContextExtended.InsertTempExpressions(tempTableType, tempTableExpressionsql);
                 
                return dbContextExtended as T;
            }
        }
    }
}
  