using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EFIntercept.Context
{
    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            AddInterceptor(new QueryInterceptor());
        }
    }
}