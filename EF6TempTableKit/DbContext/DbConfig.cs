using System.Data.Entity;

namespace EF6TempTableKit.DbContext
{
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            AddInterceptor(new QueryInterceptor());
        }
    }
}