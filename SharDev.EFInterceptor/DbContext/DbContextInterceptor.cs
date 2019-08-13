using System;
using System.Data.Entity;

namespace SharDev.EFInterceptor.DbContext
{
    [DbConfigurationType(typeof(DbConfig))]
    public class DbContextInterceptor : System.Data.Entity.DbContext
    {
        public DbContextInterceptor(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public Func<DbContextInterceptor, string, string> Method { get; set; }
    }
}
