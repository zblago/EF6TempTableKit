using EF6TempTableKit.Extensions;
using EFIntercept.Context;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace EFIntercept.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var adventureWorksDW2008R2Entities = new AdventureWorksDW2008R2Entities())
            {
                Database.SetInitializer<AdventureWorksDW2008R2Entities>(null);

                var a = 1;
                var b = 2;
                string c1 = "3";

                var q = FirstExpression(adventureWorksDW2008R2Entities, a, b, c1);
                var listResseler = adventureWorksDW2008R2Entities
                    .WithTempTableExpression<AdventureWorksDW2008R2Entities>(q)
                    .DimReseller.Join(adventureWorksDW2008R2Entities.TemporaryStudents,
                    c => c.ResellerKey,
                    c => c.Id,
                    (reseller, student) =>
                    new
                    {
                        Id = reseller.ResellerKey,
                        Name = reseller.ResellerName,
                    }).ToList();

                var t = adventureWorksDW2008R2Entities.DimReseller.First();
                
            }

            return View();
        }

        public IQueryable<TemporaryStudentIdentityDto> FirstExpression(DbContext context, int a, int b, string c)
        {
            var myContext = context as AdventureWorksDW2008R2Entities;
            var resselersQuery = myContext.DimReseller.Where(t => t.FirstOrderYear == 4).Select(x => new TemporaryStudentIdentityDto
            {
                Name = x.YearOpened.ToString(),
                Id = (int)x.FirstOrderYear,
            });

            return resselersQuery;
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