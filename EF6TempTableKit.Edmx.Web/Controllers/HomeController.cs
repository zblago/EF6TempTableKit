using EF6TempTableKit.Edmx.Web.Models.TempTables;
using EF6TempTableKit.Edmx.Web.Models;
using EF6TempTableKit.Extensions;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace EF6TempTableKit.Edmx.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new AdventureWorksEntities())
            {
                Database.SetInitializer<AdventureWorksEntities>(null);
                var tempAddressQuery = context.Address.Select(a => new AddressDto { Id = a.AddressID, Name = a.AddressLine1 });

                context
                    .WithTempTableExpression<AdventureWorksEntities>(tempAddressQuery)
                    .Address.Join(context.AddressTempTable,
                                    a => a.AddressID,
                                    at => at.Id,
                                    (a, at) =>
                                    new
                                    {
                                        Id = a.AddressID,
                                        Name = at.Name,
                                    }).ToList();

                ViewBag.EF6TempTableKitTestMessage = "EF6TempTableKit.Passed.OK";
            }

            return View();
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