using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using EF6TempTableKit.DbContext;
using EF6TempTableKit.Extensions;

namespace EF6TempTableKit.Utilities
{
    internal class TempTableDependencyManager
    {
        private readonly string _sqlSelectQuery;
        private readonly string[] _tablesUsedInQuery;
        private readonly string[] _tempTablesFromInternalStorage;
        private TempTableContainer _tempTableContainer;
        private readonly ObjectQuery _objectQuery;

        internal TempTableDependencyManager(string sqlSelectQuery, ObjectQuery objectQuery, TempTableContainer tempTableContainer)
        {
            _sqlSelectQuery = sqlSelectQuery;
            _objectQuery = objectQuery;
            _tempTableContainer = tempTableContainer;
            _tablesUsedInQuery = GetAllTablesInQuery();
            _tempTablesFromInternalStorage = _tempTableContainer.TempSqlQueriesList.Cast<System.Collections.DictionaryEntry>().Select(aaTT => aaTT.Key.ToString()).ToArray();
        }

        /// <summary>
        /// Use all tables from attached query being used for generate temp table and compare them with already attached temp tables.
        /// Get match into a separate collection called newTableDependencies.
        /// </summary>
        /// <param name="newTempTableName"></param>
        public void AddDependencyTreeForTable(string newTempTableName)
        {
            //newTempTable = key
            var firstLevelTableDependencies = _tempTablesFromInternalStorage
                .Where(aaTT => _tablesUsedInQuery.Any(tiQ => tiQ == aaTT))
                .Select(aaTT => aaTT)
                .ToArray();

            var hasDependecies = firstLevelTableDependencies.Length > 0;
            if (hasDependecies)
            {
                var allLevelsDependencies = FindNestedDependencies(firstLevelTableDependencies);

                _tempTableContainer.TempOnTempDependencies.Add(new KeyValuePair<string, string[]>(newTempTableName, allLevelsDependencies));

                System.Diagnostics.Debug.WriteLine(newTempTableName + " " + string.Join(",", allLevelsDependencies));
            }
        }

        private string[] FindNestedDependencies(string[] tableDependencies)
        {
            foreach (var item in tableDependencies)
            {
                if (_tempTableContainer.TempOnTempDependencies.ContainsKey(item))
                {
                    var nestedDependencies = FindNestedDependencies(_tempTableContainer.TempOnTempDependencies[item]);

                    var nestedDependeciesList = nestedDependencies.ToList();
                    nestedDependeciesList.AddRange(tableDependencies);

                    return nestedDependeciesList.ToArray();
                }
            }
            return tableDependencies;
        }
    }
}
