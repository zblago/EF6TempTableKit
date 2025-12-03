using EF6TempTableKit.DbContext;
using System.Data.Entity;

namespace EF6TempTableKitNET8.Test.DbContextConfiguration
{
    public class CustomDbContextConfiguration : DbConfiguration
    {
        public CustomDbContextConfiguration()
        {
            AddInterceptor(new AdventureWorkQueryInterceptor());
            AddInterceptor(new RecordFinalQueryInterceptor());
            AddInterceptor(new EF6TempTableKitQueryInterceptor());
        }
    }
}
