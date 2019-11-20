using System.Data.Entity;

namespace EF6TempTableKit.DbContext
{
    public class EF6TempTableKitDbConfiguration : DbConfiguration
    {
        public EF6TempTableKitDbConfiguration()
        {
            AddInterceptor(new EF6TempTableKitQueryInterceptor());
        }
    }
}