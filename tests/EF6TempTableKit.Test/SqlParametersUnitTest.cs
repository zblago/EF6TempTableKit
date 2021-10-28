using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EF6TempTableKit.Test
{
    public class SqlParametersUnitTest
    {
        DateTime departmentModifiedDateParam = new DateTime(2008, 4, 30, 0, 0, 0, 0); //value in database


        [Fact]
        public void WhereClauseWithMoreThan10Parameters_CompiledAndExecutedSuccesfully()
        {
            var p0 = "dsfgsdf gsdfg sdfg sdfg sdfgsdfg sdfg";
            var p1 = "test";
            var p2 = "test";
            var p3 = "test";
            var p4 = "test";
            var p5 = "test";
            var p6 = "test";
            var p7 = "test";
            var p8 = "test";
            var p9 = "test";
            var p10 = "test";
            var p11 = 1;
            var p12 = DateTime.Now;
            var falseParam13 = false;
            var p14 = 1;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            using (var context = new AdventureWorksCodeFirst())
            {

                var departmentQuery = context.Departments
                                            .Where(x =>
                                                    x.Name == p0 ||
                                                    x.Name == p1 ||
                                                    x.Name == p2 ||
                                                    x.Name == p3 &&
                                                    x.Name == p4 ||
                                                    x.Name == p5 &&
                                                    x.Name == p6 ||
                                                    (x.Name == p7 ||
                                                    x.Name == p8 &&
                                                    x.Name == p9) ||
                                                    x.Name == p10 ||
                                                    x.DepartmentID < p11 ||
                                                    x.ModifiedDate > p12 &&
                                                    true == falseParam13 &&
                                                    x.DepartmentID < p14
                                        ).Select(x => new DepartmentTempTableDto()
                                        {
                                            Name = x.Name,
                                            DepartmentId = x.DepartmentID
                                        });

                var temp = context.WithTempTableExpression<AdventureWorksCodeFirst>(departmentQuery, false).TempDepartments;

               
                var result = temp.ToList();
                Assert.True(!result.Any());
            }
        }

        [Fact]
        public void JoinTempTableWithContextTable_DontReplaceParamsWithExactValues_CompiledAndExecutedSuccesfully()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var p0 = "Shipping and Receiving";
            var p1 = "Shipping and Receiving";
            var p2 = "Shipping and Receiving";
            var p3 = "Shipping and Receiving";
            var p4 = "Shipping and Receiving";
            var p5 = "Shipping and Receiving";
            var p6 = "Shipping and Receiving";
            var p7 = "Shipping and Receiving";
            var p8 = "Shipping and Receiving";
            var p9 = "Shipping and Receiving";
            var p10 = "Shipping and Receiving";
            var p11 = 15; //departmentId in DB
            var p12 = DateTime.Now;
            var falseParam13 = false;
            var p14 = 15;//departmentId in DB


            using (var context = new AdventureWorksCodeFirst())
            {

                var departmentQuery = context.Departments
                                            .Where(x =>
                                                    x.Name == p0 &&
                                                    x.Name == p1 &&
                                                    x.Name == p2 &&
                                                    x.Name == p3 &&
                                                    x.Name == p4 &&
                                                    x.Name == p5 &&
                                                    x.Name == p6 &&
                                                    x.Name == p7 &&
                                                    x.Name == p8 &&
                                                    x.Name == p9 &&
                                                    x.Name == p10 &&
                                                    x.DepartmentID == p11 &&
                                                    x.ModifiedDate == departmentModifiedDateParam &&
                                                    false == falseParam13 &&
                                                    x.DepartmentID == p14
                                        ).Select(x => new DepartmentTempTableDto()
                                        {
                                            Name = x.Name,
                                            DepartmentId = x.DepartmentID
                                        });

                //this doesn work, because sql params are not sent

                /*generated sql 

                DECLARE @tempTable#tempDepartmentCreated bit = 0
IF OBJECT_ID(''tempdb..#tempDepartment'') IS NOT NULL
BEGIN

    SET @tempTable#tempDepartmentCreated = 1
	DROP TABLE #tempDepartment
END

CREATE TABLE #tempDepartment
(
    DepartmentId int,
    Name varchar(200),
    LeadId int
)
INSERT INTO #tempDepartment(Name, DepartmentId) 
SELECT Name, DepartmentId FROM
(SELECT
    1 AS[C1],
    [Extent1].[Name] AS[Name],
     CAST( [Extent1].[DepartmentID] AS int) AS[C2]
    FROM[HumanResources].[Department] AS[Extent1]
    WHERE([Extent1].[Name] = @p__linq__0) AND([Extent1].[Name] = @p__linq__1) AND([Extent1].[Name] = @p__linq__2) AND([Extent1].[Name] = @p__linq__3) AND([Extent1].[Name] = @p__linq__4) AND([Extent1].[Name] = @p__linq__5) AND([Extent1].[Name] = @p__linq__6) AND([Extent1].[Name] = @p__linq__7) AND([Extent1].[Name] = @p__linq__8) AND([Extent1].[Name] = @p__linq__9) AND([Extent1].[Name] = @p__linq__10) AND(CAST( [Extent1].[DepartmentID] AS int) = @p__linq__11) AND([Extent1].[ModifiedDate] = @p__linq__12) AND(0 = @p__linq__13) AND(CAST( [Extent1].[DepartmentID] AS int) = @p__linq__14)) AS aliastempDepartment(TempColumn, Name, DepartmentId)


SELECT
    [Extent2].[DepartmentID] AS[DepartmentID], 
    [Extent2].[Name] AS[Name], 
    [Extent2].[GroupName] AS[GroupName], 
    [Extent2].[ModifiedDate]
        AS[ModifiedDate]
FROM[dbo].[#tempDepartment] AS [Extent1]
    INNER JOIN [HumanResources].[Department] AS[Extent2] ON[Extent1].[DepartmentId] =  CAST([Extent2].[DepartmentID] AS int)
    WHERE 1 = @p__linq__0',N'@p__linq__0 bit',@p__linq__0=1  -- <== HERE, PARAMS ARE NOT INJECTED

                 * */
                var temp = context.WithTempTableExpression<AdventureWorksCodeFirst>(departmentQuery, false, replaceParamsWithValues: false).TempDepartments;

                var trueBool = true;
                var query = from tempDepartment in temp
                            join department in context.Departments on tempDepartment.DepartmentId equals department.DepartmentID
                            where trueBool == true
                            select department;
              

                var result = query.ToList();
                Assert.True(result.Any());
            }
        }

        [Fact]
        public void WhereClauseWithBooleanParameter_CompiledAndExecutedSuccesfully()
        {
            var boolParameter = false;

            using (var context = new AdventureWorksCodeFirst())
            {

                var departmentQuery = context.Departments
                                            .Where(x => true == boolParameter)
                                            .Select(x => new DepartmentTempTableDto()
                                            {
                                                Name = x.Name
                                            });

                var temp = context.WithTempTableExpression<AdventureWorksCodeFirst>(departmentQuery, false).TempDepartments;
                var result = temp.ToList();
                Assert.True(!result.Any());
            }

        }

        [Fact]
        public void WhereClauseWithDateTimeParameter_CompiledAndExecutedSuccesfully()
        {
            var dateTimeNowParam = DateTime.Now;
            var yesterday = DateTime.Now.AddDays(-1);

            using (var context = new AdventureWorksCodeFirst())
            {

                var departmentQuery = context.Departments
                                            .Where(x => yesterday == dateTimeNowParam)
                                            .Select(x => new DepartmentTempTableDto()
                                            {
                                                Name = x.Name
                                            });

                var temp = context.WithTempTableExpression<AdventureWorksCodeFirst>(departmentQuery, false).TempDepartments;
                var result = temp.ToList();
                Assert.True(!result.Any());
            }
        }

        [Fact]
        public void WhereClauseWithDateTimeParameter_DontReplaceParamsWithExactValues__CompiledAndExecutedSuccesfully()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            using (var context = new AdventureWorksCodeFirst())
            {

                var departmentQuery = context.Departments
                                            .Where(x => x.ModifiedDate == departmentModifiedDateParam)
                                            .Select(x => new DepartmentTempTableDto()
                                            {
                                                Name = x.Name
                                            });

                var temp = context.WithTempTableExpression<AdventureWorksCodeFirst>(departmentQuery, false).TempDepartments;
                var result = temp.ToList();
                Assert.True(result.Any());
            }
        }

    }
}
