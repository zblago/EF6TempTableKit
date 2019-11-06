using EF6TempTableKit.DbContext;
using System.Data.Entity;

namespace EF6TempTableKit.Edmx.Web.Models
{
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            AddInterceptor(new QueryInterceptor());
        }
    }
}