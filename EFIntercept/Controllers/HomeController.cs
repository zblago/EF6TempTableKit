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
            public int? FirstOrderYear { get; set; }
            public int? YearOpened { get; set; }
        }

        public ActionResult Index()
        {
            int a = 1;
            using (var adventureWorksDW2008R2Entities = new AdventureWorksDW2008R2Entities())
            {
                var listReseller = adventureWorksDW2008R2Entities
                    .WithCustomQuery<AdventureWorksDW2008R2Entities>(ModifiyCommandText)
                    .DimReseller.Where(t => t.FirstOrderYear == a).ToList();
            }

            return View();
        }

        //custom code - user can change it
        public string ModifiyCommandText(DbContextInterceptor context, string commandText)
        {
            var myContext = context as AdventureWorksDW2008R2Entities;
            var resselersQuery = myContext.DimReseller.Where(t => t.FirstOrderYear == 4).Select(x => new TempTable { FirstOrderYear = x.FirstOrderYear, YearOpened = x.YearOpened });
            var sql = (resselersQuery as System.Data.Entity.Infrastructure.DbQuery<TempTable>).Sql.Replace("1 AS [C1],", "");

            var tempTableCreator = new TempTableCreator();
            var tempTableWithQuery = tempTableCreator.CreateTempTable(new List<Func<string>>
                {
                    () => { return "FirstOrderYear int"; },
                    () => { return "YearOpened int"; },
                }, "tempTest").Insert(sql);

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