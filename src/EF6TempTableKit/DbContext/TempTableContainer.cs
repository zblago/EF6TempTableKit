using System.Collections.Generic;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new Dictionary<string, Query>();
        }

        public IDictionary<string, Query> TempSqlQueriesList { set; get; }
    }
}