using System.Data.Entity;

namespace SharDev.EFInterceptor.DbContext
{
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            AddInterceptor(new QueryInterceptor());
        }
    }
}