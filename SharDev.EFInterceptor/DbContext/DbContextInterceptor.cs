using SharDev.EFInterceptor.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SharDev.EFInterceptor.DbContext
{
    [DbConfigurationType(typeof(DbConfig))]
    public class DbContextInterceptor : System.Data.Entity.DbContext
    {
        public IList<IQueryable<ITempTable>> TempExpressionsList { private set; get; }

        public DbContextInterceptor(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            TempExpressionsList = new List<IQueryable<ITempTable>>();
        }

        public Func<DbContextInterceptor, string, string> Method { get; set; }

        public void InsertTempExpressions(IQueryable<ITempTable> expression)
        {
            TempExpressionsList.Add(expression);
        } 
    }
}
