﻿using EF6TempTableKit.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF6TempTableKit.Utilities
{
    /// <summary>
    /// Queries attached on a context are stored in a queue(FIFO) data structure. Why? Because last attached query typically has dependencies on previously attached queries.
    /// Furthermore, every iteration element (key - table name, value - query objext) has dependencies on some other temp tables stored in flatten list (TempOnTempDependencies) sorted from those who have a lot of dependencies to those with one or zero dependencies.
    /// </summary>
    internal sealed class SqlFromTempTableDependenciesBuilder
    {
        private readonly Version _assemblyVersion;
        private readonly string _generatedByEf6TempTableKitStartMsg;
        private readonly string _generatedByEf6TempTableKitEndMsg;
        private readonly IDictionary<string, HashSet<string>> _tempOnTempDependencies;
        private readonly Queue<KeyValuePair<string, Query>> _tempSqlQueriesList;

        internal SqlFromTempTableDependenciesBuilder(TempTableContainer tempTableContainer)
        {
            _tempOnTempDependencies = tempTableContainer.TempOnTempDependencies;
            _tempSqlQueriesList = tempTableContainer.TempSqlQueriesList;

            _assemblyVersion = typeof(EF6TempTableKitQueryInterceptor).Assembly.GetName().Version;
            _generatedByEf6TempTableKitStartMsg = $"/* Generated by EF6TempTableKit {_assemblyVersion} - START */";
            _generatedByEf6TempTableKitEndMsg = $"/* Generated by EF6TempTableKit {_assemblyVersion} - END*/";
        }

        public string BuildSqlForTempTables(string interceptedComandText)
        {
            var alreadyAttachedTempTableQuery = new HashSet<string>();
            var sqlStringBuilder = new StringBuilder();

            foreach (var tempSqlQuery in _tempSqlQueriesList)
            {
                var tempTableName = tempSqlQuery.Key;
                var tempTableNameWithBrackets = $"[{tempTableName}]";
                if (!alreadyAttachedTempTableQuery.Any(t => t == tempTableName) && interceptedComandText.Contains(tempTableNameWithBrackets))
                {
                    var hasTempTableDependencies = _tempOnTempDependencies.ContainsKey(tempTableName);
                    if (hasTempTableDependencies)
                    {
                        foreach (var tempTable in _tempOnTempDependencies[tempTableName])
                        {
                            var query = _tempSqlQueriesList.Single(t => t.Key == tempTable).Value;
                            AppendSqlString(sqlStringBuilder, query.QueryString);
                            alreadyAttachedTempTableQuery.Add(tempTable);
                        }
                    }
                    AppendSqlString(sqlStringBuilder, tempSqlQuery.Value.QueryString);
                }
            }

            return sqlStringBuilder.ToString();
        }

        private void AppendSqlString(StringBuilder sqlStringBuilder, string sqlStringToAppend)
        {
            var selectCommandTextFormat = "\n{0}\n{1}\n{2}\n";

            sqlStringBuilder.AppendFormat(selectCommandTextFormat, _generatedByEf6TempTableKitStartMsg, sqlStringToAppend, _generatedByEf6TempTableKitEndMsg);
        }
    }
}
