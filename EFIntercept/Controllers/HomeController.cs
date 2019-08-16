using EFIntercept.Context;
using SharDev.EFInterceptor.DbContext;
using SharDev.EFInterceptor.Extensions.SharkDev.Extensions;
using SharDev.EFInterceptor.SqlUtility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

                var listResseler = adventureWorksDW2008R2Entities
                    .WithCustomQuery<AdventureWorksDW2008R2Entities>(ModifiyCommandText)
                    .DimReseller.Join(adventureWorksDW2008R2Entities.TemporaryStudents,
                    c => c.ResellerKey,
                    c => c.Id,
                    (reseller, student) => 
                    new
                    {
                        reseller.ResellerKey,
                        reseller.ResellerName,
                        student.Name
                    }).ToList();
                /*
                var listReseller = adventureWorksDW2008R2Entities
                    .WithCustomQuery<AdventureWorksDW2008R2Entities>(ModifiyCommandText)
                    .DimReseller.Where(t => t.FirstOrderYear == a).ToList();
                    */
            }

            return View();
        }

        //custom code - user can change it
        public string ModifiyCommandText(DbContextInterceptor context, string commandText)
        {
            var myContext = context as AdventureWorksDW2008R2Entities;
            var resselersQuery = myContext.DimReseller.Where(t => t.FirstOrderYear == 4).Select(x => new TempTable {
                Id = (int) x.FirstOrderYear,
                Name = x.YearOpened.ToString() });
            var sql = (resselersQuery as System.Data.Entity.Infrastructure.DbQuery<TempTable>).Sql.Replace("1 AS [C1],", "");

            var tempTableCreator = new TempTableCreator();
            var tempTableWithQuery = tempTableCreator.CreateTempTable(new List<Func<string>>
                {
                    () => { return "Id int"; },
                    () => { return "Name varchar(100)"; },
                }, "tempStudent").Insert(sql);

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