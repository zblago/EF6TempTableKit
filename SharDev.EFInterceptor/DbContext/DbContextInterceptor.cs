using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace SharDev.EFInterceptor.DbContext
{
    [DbConfigurationType(typeof(DbConfig))]
    public class DbContextInterceptor : System.Data.Entity.DbContext
    {
        public IDictionary<string, string> TempSqlQueriesList { private set; get; }

        public DbContextInterceptor(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public DbContextInterceptor(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbContextInterceptor(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        public DbContextInterceptor(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            TempSqlQueriesList = new Dictionary<string, string>();
        }

        public Func<DbContextInterceptor, string, string> Method { get; set; }

        public void InsertTempExpressions(string type, string expression)
        {
            TempSqlQueriesList.Add(type, expression);
        } 
    }
}
