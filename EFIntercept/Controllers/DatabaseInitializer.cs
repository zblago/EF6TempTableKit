using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EFIntercept.Controllers
{
    public class DatabaseInitializer : IDatabaseInitializer<DbContext>
    {
        public void InitializeDatabase(DbContext context)
        {
            throw new NotImplementedException();
        }
    }
}