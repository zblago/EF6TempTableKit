using System;
using System.Collections.Generic;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new Dictionary<string, string>();
        }

        public IDictionary<string, string> TempSqlQueriesList { set; get; }

        public Func<System.Data.Entity.DbContext, string, string> Method { get; set; }
    }
}