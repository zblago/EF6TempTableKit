using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace EF6TempTableKit.DbContext
{
    [DbConfigurationType(typeof(DbConfig))]
    public class DbContextWithTempTable : System.Data.Entity.DbContext
    {
        public IDictionary<string, string> TempSqlQueriesList { private set; get; }

        public Func<DbContextWithTempTable, string, string> Method { get; set; }

        public DbContextWithTempTable(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public DbContextWithTempTable(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbContextWithTempTable(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        public DbContextWithTempTable(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            TempSqlQueriesList = new Dictionary<string, string>();
        }

        public void InsertTempExpressions(string type, string expression)
        {
            TempSqlQueriesList.Add(type, expression);
        }
    }
}
