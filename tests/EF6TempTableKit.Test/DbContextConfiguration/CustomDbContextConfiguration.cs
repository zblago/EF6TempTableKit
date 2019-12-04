using EF6TempTableKit.DbContext;
using System.Data.Entity;

namespace EF6TempTableKit.Test.DbContextConfiguration
{
    public class CustomDbContextConfiguration : DbConfiguration
    {
        public CustomDbContextConfiguration()
        {
            AddInterceptor(new AdventureWorkQueryInterceptor());
            AddInterceptor(new EF6TempTableKitQueryInterceptor());
        }
    }
}
