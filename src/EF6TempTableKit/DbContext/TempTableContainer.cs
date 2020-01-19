using EF6TempTableKit.Models;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new OrderedDictionary();
            TempOnTempDependencies = new Dictionary<string, IEnumerable<TempTableInQuery>>();
        }

        public IOrderedDictionary TempSqlQueriesList { set; get; }
        internal IDictionary<string, IEnumerable<TempTableInQuery>> TempOnTempDependencies { get; set; }
    }
}