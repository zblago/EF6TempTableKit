using System.Collections.Generic;
using System.Collections.Specialized;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new OrderedDictionary();
            TempOnTempDependecies = new Dictionary<string, string[]>();
        }

        public IOrderedDictionary TempSqlQueriesList { set; get; }
        public IDictionary<string, string[]> TempOnTempDependecies { get; set; }
    }
}