using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.Web.Model.TempTables;
using EF6TempTableKit.Test.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EF6TempTableKit.Test.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new AdventureWorks())
            {
                //Database.SetInitializer<AdventureWorksDW2008R2Entities>(null); //Obviously not needed

                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto
                {
                    Id = a.AddressID,
                    AddressLine1 = a.AddressLine1
                });

                var joinedAddress = context
                        .WithTempTableExpression<AdventureWorks>(tempAddressQuery)
                        .AddressesTempTable.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => new { Id = at.Id }).ToList();

                ViewBag.EF6TempTableKitResult = "EF6TempTableKit.Passed.OK";    
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