using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace EF6TempTableKit.DbContext
{
    [DbConfigurationType(typeof(DbConfig))]
    public class DbContextWithTempTable : System.Data.Entity.DbContext
    {
        public IDictionary<string, string> TempSqlQueriesList { private set; get; }

        public Func<DbContextWithTempTable, string, string> Method { get; set; }

        public DbContextWithTempTable(string nameOrConnectionString) 
            : base(nameOrConnectionString)
        {
            TempSqlQueriesList = new Dictionary<string, string>();
        }

        public DbContextWithTempTable(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        public DbContextWithTempTable(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbContextWithTempTable(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        public DbContextWithTempTable(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base (existingConnection, model, contextOwnsConnection)
        {
        }

        public void InsertTempExpressions(string type, string expression)
        {
            TempSqlQueriesList.Add(type, expression);
        }
    }
}
