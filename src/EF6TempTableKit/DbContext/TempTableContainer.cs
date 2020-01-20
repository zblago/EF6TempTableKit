using System.Collections.Generic;
using System.Collections.Specialized;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new OrderedDictionary();
            TempOnTempDependencies = new Dictionary<string, HashSet<string>>();
        }

        public IOrderedDictionary TempSqlQueriesList { set; get; }
        internal IDictionary<string, HashSet<string>> TempOnTempDependencies { get; set; }
    }
}