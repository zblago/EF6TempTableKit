using EF6TempTableKit.DbContext;
using System.Data.Entity;

namespace EF6TempTableKit.Test.Interceptors
{
    public class MyContextDbConfiguration : DbConfiguration
    {
        public MyContextDbConfiguration()
        {
            AddInterceptor(new AdventureWorkQueryInterceptor());
            AddInterceptor(new EF6TempTableKitQueryInterceptor());
        }
    }
}