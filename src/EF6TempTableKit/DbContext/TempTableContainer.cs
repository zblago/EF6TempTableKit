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

        /// <summary>
        /// Key is node. Value are children.
        /// Children are like this - at the top is an item that has dependecies to the items below. Last item has no any dependecies
        /// </summary>
        internal IDictionary<string, HashSet<string>> TempOnTempDependencies { get; set; }
    }
}