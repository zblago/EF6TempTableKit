using EF6TempTableKit.Test.Web.Entities;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EF6TempTableKit.Test.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            PrepareEF6TempTableKit();
        }

        private void PrepareEF6TempTableKit()
        {
            //Database.SetInitializer<AdventureWorks>(null); //Obviously not needed
        }
    }
}
