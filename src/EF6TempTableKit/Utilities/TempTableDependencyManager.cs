using EF6TempTableKit.DbContext;
using System;
using System.Collections.Generic;

namespace EF6TempTableKit.Utilities
{
    internal class TempTableDependencyManager
    {
        private readonly string _sqlSelectQuery;
        private TempTableContainer _tempTableContainer;

        internal TempTableDependencyManager(string sqlSelectQuery, TempTableContainer tempTableContainer)
        {
            _sqlSelectQuery = sqlSelectQuery;
            _tempTableContainer = tempTableContainer;
        }

        public void AddDependency(string newTempTableName)
        {
            var addedTempTables = _tempTableContainer.TempSqlQueriesList.Keys;
            foreach (var addedTempTable in addedTempTables)
            {
                var addedTempTableString = addedTempTable.ToString();

                //addedTempTableString : table on which newTableName depends
                //newTempTableName : table that depends on addedTempTableString

                var neededInInsertQuery = _sqlSelectQuery.Contains(addedTempTableString);
                if (neededInInsertQuery)
                {
                    if (!_tempTableContainer.TempOnTempDependencies.ContainsKey(addedTempTableString))
                    {
                        _tempTableContainer.TempOnTempDependencies.Add(new KeyValuePair<string, string[]>(addedTempTableString, new string[] { newTempTableName }));
                    }
                    else
                    {
                        var values = _tempTableContainer.TempOnTempDependencies[addedTempTableString];

                        Array.Resize(ref values, values.Length + 1);

                        var lastIndex = values.Length - 1;
                        values[lastIndex] = newTempTableName;

                        _tempTableContainer.TempOnTempDependencies[addedTempTableString] = values;
                    }
                }
            }
        }
    }
}
