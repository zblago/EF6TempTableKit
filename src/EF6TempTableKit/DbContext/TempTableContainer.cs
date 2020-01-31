using System.Collections.Generic;

namespace EF6TempTableKit.DbContext
{
    public sealed class TempTableContainer
    {
        public TempTableContainer()
        {
            TempSqlQueriesList = new Queue<KeyValuePair<string, Query>>();
            TempOnTempDependencies = new Dictionary<string, HashSet<string>>();
        }

        public Queue<KeyValuePair<string, Query>> TempSqlQueriesList { set; get; }

        /// <summary>
        /// Key is node. 
        /// Value are children - at the top is an item that has dependencies on the items below. Last item has no any dependencies.
        /// </summary>
        internal IDictionary<string, HashSet<string>> TempOnTempDependencies { get; set; }
    }
}