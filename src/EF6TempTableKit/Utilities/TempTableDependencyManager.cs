using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.Extensions;
using EF6TempTableKit.Models;

namespace EF6TempTableKit.Utilities
{
    internal class TempTableDependencyManager
    {
        private readonly string _sqlSelectQuery;
        private readonly string[] _tablesUsedInQuery;
        private readonly string[] _tempSqlQueryList;
        private TempTableContainer _tempTableContainer;
        private readonly ObjectQuery _objectQuery;

        internal TempTableDependencyManager(string sqlSelectQuery, ObjectQuery objectQuery, TempTableContainer tempTableContainer)
        {
            _sqlSelectQuery = sqlSelectQuery;
            _objectQuery = objectQuery;
            _tempTableContainer = tempTableContainer;
            _tablesUsedInQuery = GetAllTablesInQuery();
            _tempSqlQueryList = _tempTableContainer.TempSqlQueriesList.Cast<System.Collections.DictionaryEntry>().Select(aaTT => aaTT.Key.ToString()).ToArray();
        }

        /// <summary>
        /// Use all tables from attached query and compare with already attached temp tables.
        /// Get match into a separate collection. Traverse through the all leafs to construct dependencies.
        /// </summary>
        /// <param name="newTempTableName"></param>
        public void AddDependencyTreeForTable(string newTempTableName)
        {
            //newTempTableName = key
            var firstLevelTableDependencies = _tempSqlQueryList
                .Where(aaTT => _tablesUsedInQuery.Any(tiQ => tiQ == aaTT))
                .Select(aaTT => aaTT)
                .ToArray();

            var hasDependeciesAtFirstLevel = firstLevelTableDependencies.Length > 0;
            if (hasDependeciesAtFirstLevel)
            {
                var allLevelsDependencies = FindNestedDependencies(firstLevelTableDependencies.Select(fl => new TempTableInQuery { Name = fl }));

                _tempTableContainer.TempOnTempDependencies.Add(new KeyValuePair<string, IEnumerable<TempTableInQuery>>(newTempTableName, allLevelsDependencies));

                System.Diagnostics.Debug.WriteLine(newTempTableName + " " + string.Join(",", allLevelsDependencies.Select(a => a.Name)));
            }
        }

        private IEnumerable<TempTableInQuery> FindNestedDependencies(IEnumerable<TempTableInQuery> tableDependencies)
        {
            foreach (var item in tableDependencies)
            {
                if (_tempTableContainer.TempOnTempDependencies.ContainsKey(item.Name))
                {
                    var nestedDependencies = FindNestedDependencies(_tempTableContainer.TempOnTempDependencies[item.Name]);

                    var nestedDependeciesList = nestedDependencies.ToList();
                    nestedDependeciesList.AddRange(tableDependencies);

                    return nestedDependeciesList.ToArray();
                }
            }
            return tableDependencies;
        }

        private string[] GetAllTablesInQuery()
        {
            return _objectQuery.GetTables().Where(t => t.Table != null).Select(t => t.Table).ToArray(); //Take all as we can't filter out (easily) only ITempTable
        }
    }
}
