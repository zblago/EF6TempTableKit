using EFIntercept.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SharkDev.Extensions
{
    //generic code
    public static class DbContextExtensions
    {
        public static T WithCustomQuery<T>(this DbContextExtended dbContextExtended, Func<DbContextExtended, string, string> method) where T : class
        {
            dbContextExtended.Method = method;

            return dbContextExtended as T;
        }
    }
}

namespace EFIntercept.Controllers
{
    using SharkDev.Extensions;

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
            using (var adventureWorksDW2008R2Entities = new AdventureWorksDW2008R2EntitiesCustomized())
            {
                var listReseller = adventureWorksDW2008R2Entities
                    .WithCustomQuery<AdventureWorksDW2008R2EntitiesCustomized>(ModifiyCommandText)
                    .DimReseller.Where(t => t.FirstOrderYear == a).ToList();
            }

            return View();
        }

        //custom code - user can change it
        public string ModifiyCommandText(DbContextExtended context, string commandText)
        {
            var myContext = context as AdventureWorksDW2008R2EntitiesCustomized;
            var resselersQuery = myContext.DimReseller.Where(t => t.FirstOrderYear == 4).Select(x => new TempTable { FirstOrderYear = x.FirstOrderYear, YearOpened = x.YearOpened });
            var sql = (resselersQuery as System.Data.Entity.Infrastructure.DbQuery<TempTable>).Sql.Replace("1 AS [C1],", "");

            var commandTextWithTempTable = InsertIntoTempTable(new List<Func<string>>
                {
                    () => { return "FirstOrderYear int"; },
                    () => { return "YearOpened int"; },
                }, "tempTest", sql);

            commandText = commandTextWithTempTable + sql;

            return commandText;
        }

        //generic code
        private string InsertIntoTempTable(List<Func<string>> map, string tempTableName, string query)
        {
            var tempTableCreate = "CREATE TABLE #" + tempTableName + " (";
            var fieldsDeclaration = "";
            map.ForEach(m =>
            {
                fieldsDeclaration = fieldsDeclaration + m() + ",";
            });
            tempTableCreate = tempTableCreate + fieldsDeclaration.Remove(fieldsDeclaration.Length - 1, 1) + ")";

            return tempTableCreate + ";\n\n" + "INSERT INTO #" + tempTableName + "\n" + query + ";\n\n";
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