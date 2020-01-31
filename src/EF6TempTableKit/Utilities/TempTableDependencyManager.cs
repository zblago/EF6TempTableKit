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
        private readonly string _sqlSelectQuery;
        private readonly string[] _tablesUsedInQuery;
        private readonly string[] _tempSqlQueryList;
        private readonly TempTableContainer _tempTableContainer;
        private readonly ObjectQuery _objectQuery;

        internal TempTableDependencyManager(string sqlSelectQuery, ObjectQuery objectQuery, TempTableContainer tempTableContainer)
        {
            _sqlSelectQuery = sqlSelectQuery;
            _objectQuery = objectQuery;
            _tempTableContainer = tempTableContainer;
            _tablesUsedInQuery = GetAllTablesInQuery();
            _tempSqlQueryList = _tempTableContainer.TempSqlQueriesList.Select(t => t.Key).ToArray();
        }

        /// <summary>
        /// Use all tables from attached query and compare with already attached temp tables.
        /// Get match into a separate collection. Traverse through the all nodes to get their children.
        /// </summary>
        /// <param name="newTempTableName"></param>
        public void AddDependenciesForTable(string newTempTableName)
        {
            //newTempTableName = key
            var firstLevelChildren = _tempSqlQueryList
                .Where(aaTT => _tablesUsedInQuery.Any(tiQ => tiQ == aaTT))
                .Select(aaTT => aaTT)
                .ToArray();

            var firstLeveNodeHasChildren = firstLevelChildren.Length > 0;
            if (firstLeveNodeHasChildren)
            {
                _tempTableContainer
                    .TempOnTempDependencies
                    .Add(new KeyValuePair<string, HashSet<string>>(newTempTableName, new HashSet<string>()));

                var childrenDependecies = new List<string>();
                foreach (var item in firstLevelChildren)
                {
                    if (_tempTableContainer.TempOnTempDependencies.ContainsKey(item))
                    {
                        childrenDependecies.AddRange(_tempTableContainer.TempOnTempDependencies[item]);
                        childrenDependecies.Add(item);
                        childrenDependecies.ForEach(cd => _tempTableContainer.TempOnTempDependencies[newTempTableName].AddIfNotExists(cd));
                    }
                    else
                    {
                        _tempTableContainer.TempOnTempDependencies[newTempTableName].AddIfNotExists(item);
                    }
                }

                System.Diagnostics.Debug.WriteLine(newTempTableName + " " + string.Join(",", _tempTableContainer.TempOnTempDependencies[newTempTableName]));
            }
        }

        private string[] GetAllTablesInQuery()
        {
            return _objectQuery.GetTables().Where(t => t.Table != null).Select(t => t.Table).ToArray(); //Take all as we can't filter out (easily) only ITempTable
        }
    }
}
