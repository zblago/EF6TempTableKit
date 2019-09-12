using EF6TempTableKit.Model;
using System;
using System.Collections.Generic;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new Dictionary<string, QueryString>();
        }

        public IDictionary<string, QueryString> TempSqlQueriesList { set; get; }
    }
}