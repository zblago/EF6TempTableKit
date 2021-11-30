using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.Extensions;

namespace EF6TempTableKit.Utilities
{
    /// <summary>
    /// Handles dependencies in a form of a tree. Root node doesn't exist - think of it as something imaginary.
    /// Node - represent parent of children nodes.
    /// Children - list of nodes that belongs to parent node.
    /// Level - only two levels 
    ///     1. parent (temp table)
    ///     2. children(dependencies - tables on which parent depends).
    /// </summary>
    internal class TempTableDependencyManager
    {
        private readonly string[] _tablesUsedInQuery;
        private readonly string[] _tempSqlQueryList;
        private readonly TempTableContainer _tempTableContainer;
        private readonly ObjectQuery _objectQuery;

        internal TempTableDependencyManager(ObjectQuery objectQuery, TempTableContainer tempTableContainer)
        {
            _objectQuery = objectQuery;
            _tempTableContainer = tempTableContainer;
            _tablesUsedInQuery = GetAllTablesInQuery();
            _tempSqlQueryList = _tempTableContainer.TempSqlQueriesList.Select(t => t.Key).ToArray();
        }

        /// <summary>
        /// Use all tables from attached query and compare with already attached temp tables.
        /// Get match into a separate collection. Traverse through the first level children as they already have dependencies.
        /// </summary>
        /// <param name="newTempTableName"></param>
        public void AddDependenciesForTable(string newTempTableName)
        {
            //newTempTableName = key
            var alreadyAttachedTempTablesFromQuery = _tempSqlQueryList
                .Where(aaTT => _tablesUsedInQuery.Any(tiQ => tiQ == aaTT))
                .Select(aaTT => aaTT)
                .ToArray();

            var hasAlreadyAttachedTempTablesFromQuery = alreadyAttachedTempTablesFromQuery.Length > 0;
            if (hasAlreadyAttachedTempTablesFromQuery)
            {
                _tempTableContainer
                    .TempOnTempDependencies
                    .Add(new KeyValuePair<string, HashSet<string>>(newTempTableName, new HashSet<string>()));

                var childrenDependencies = new List<string>();
                foreach (var item in alreadyAttachedTempTablesFromQuery)
                {
                    if (_tempTableContainer.TempOnTempDependencies.ContainsKey(item))
                    {
                        childrenDependencies.AddRange(_tempTableContainer.TempOnTempDependencies[item]);
                        childrenDependencies.Add(item);
                        childrenDependencies.ForEach(cd => _tempTableContainer.TempOnTempDependencies[newTempTableName].AddIfNotExists(cd));
                    }
                    else
                    {
                        _tempTableContainer.TempOnTempDependencies[newTempTableName].AddIfNotExists(item);
                    }
                }
            }
        }

        private string[] GetAllTablesInQuery()
        {
            return _objectQuery.GetTables().Where(t => t.Table != null).Select(t => t.Table).ToArray(); //Take all as we can't filter out (easily) only ITempTable
        }
    }
}
