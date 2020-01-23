using System.Collections.Generic;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new Stack<KeyValuePair<string, Query>>();
            TempOnTempDependencies = new Dictionary<string, HashSet<string>>();
        }

        public Stack<KeyValuePair<string, Query>> TempSqlQueriesList { set; get; }

        /// <summary>
        /// Key is node. 
        /// Value are children - at the top is an item that has dependecies on the items below. Last item has no any dependecies.
        /// </summary>
        internal IDictionary<string, HashSet<string>> TempOnTempDependencies { get; set; }
    }
}