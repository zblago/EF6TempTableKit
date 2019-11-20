using MvcIntegrationTestFramework.Browsing;
using MvcIntegrationTestFramework.Hosting;
using Xunit;

namespace EF6TempTableKit.Test
{
    public class IntegrationTest
    {
        [Fact]
        public void LoadAddressList()
        {
            var apphost = AppHost.Simulate("EF6TempTableKit.Test.Web");
            apphost.Start(browsingSession =>
            {
                RequestResult result = browsingSession.Get("/Home/Index");
                var responseText = result.ResponseText;

                Assert.Contains("EF6TempTableKit.Passed.OK", responseText);
            });
            apphost.Dispose();
        }
    }
}
