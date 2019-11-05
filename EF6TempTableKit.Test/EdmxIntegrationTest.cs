using EF6TempTableKit.Edmx.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EF6TempTableKit.UnitTest
{
    public class EdmxIntegrationTest
    {
        [Fact]
        public void PassingTest()
        {
            var homeController = new HomeController();
            var view = homeController.Index();
            Assert.NotNull(view);
        }
    }
}
