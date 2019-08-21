using EFIntercept.Context;
using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Extensions;
using SharDev.EFInterceptor.Extensions.SharkDev.Extensions;
using SharDev.EFInterceptor.SqlUtility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace EFIntercept.Controllers
{
    public class HomeController : Controller
    {
        public class TempTable
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public ActionResult Index()
        {
            using (var adventureWorksDW2008R2Entities = new AdventureWorksDW2008R2Entities())
            {
                Database.SetInitializer<AdventureWorksDW2008R2Entities>(null);

                var a = 1;
                var b = 2;
                string c1 = "3";

                var q = FirstExpression(adventureWorksDW2008R2Entities, a, b, c1);
                var q1 = ModifiyCommandText(adventureWorksDW2008R2Entities, "");
                var listResseler = adventureWorksDW2008R2Entities
                    .WithTempExpression<AdventureWorksDW2008R2Entities>(q)
                    //.WithCustomQuery<AdventureWorksDW2008R2Entities>(ModifiyCommandText)
                    .DimReseller.Join(adventureWorksDW2008R2Entities.TemporaryStudents,
                    c => c.ResellerKey,
                    c => c.Id,
                    (reseller, student) =>
                    new
                    {
                        Id = reseller.ResellerKey,
                        Name = reseller.ResellerName,
                    }).ToList();
                
            }

            return View();
        }

        public IQueryable<TemporaryStudentIdentity> FirstExpression(DbContextInterceptor context, int a, int b, string c)
        {
            var myContext = context as AdventureWorksDW2008R2Entities;
            var resselersQuery = myContext.DimReseller.Where(t => t.FirstOrderYear == 4).Select(x => new TemporaryStudentIdentity
            {
                Name = x.YearOpened.ToString(),
                Id = (int)x.FirstOrderYear,
            });

            /*
            var resselersQuery1 = myContext.DimReseller.Where(t => t.FirstOrderYear == 4).Select(x => new TempTable
            {
                Name = x.YearOpened.ToString(),
                Id = (int)x.FirstOrderYear,
            });
            var test = resselersQuery1.ToTraceQuery<TempTable>();
            */
            return resselersQuery;
        }

        //custom code - user can change it
        public string ModifiyCommandText(DbContextInterceptor context, string commandText)
        {
            var myContext = context as AdventureWorksDW2008R2Entities;
            var a = 5;
            var resselersQuery = myContext.DimReseller.Where(t => t.FirstOrderYear == a).Select(x => new TempTable
            {
                Name = x.YearOpened.ToString(),
                Id = (int)x.FirstOrderYear,
            });
            var sql = (resselersQuery as System.Data.Entity.Infrastructure.DbQuery<TempTable>).Sql.Replace("1 AS [C1],", "");

            var tempTableCreator = new TempTableCreator();
            var tempTableWithQuery = tempTableCreator.CreateTempTable(new List<Func<string>>
                {
                    () => { return "Id int"; },
                    () => { return "Name varchar(100)"; },
                }, "#tempStudent").Insert(sql);

            commandText = tempTableWithQuery + commandText;

            return commandText;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}