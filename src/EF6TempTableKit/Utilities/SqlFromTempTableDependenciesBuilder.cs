﻿using EF6TempTableKit.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EF6TempTableKit.SqlCommands;

namespace EF6TempTableKit.Utilities
{
    /// <summary>
    /// Queries attached on a context are stored in a queue(FIFO) data structure. Why? Because last attached query typically has dependencies on previously attached queries.
    /// </summary>
    internal sealed class SqlFromTempTableDependenciesBuilder
    {
        private readonly Version _assemblyVersion;
        private readonly string _generatedByEf6TempTableKitStartMsg;
        private readonly string _generatedByEf6TempTableKitEndMsg;
        private readonly IDictionary<string, HashSet<string>> _tempOnTempDependencies;
        private readonly Queue<KeyValuePair<string, Query>> _tempSqlQueriesList;
        private HashSet<string> _alreadyAttachedTempTableQuery;

        internal SqlFromTempTableDependenciesBuilder(TempTableContainer tempTableContainer)
        {
            _tempOnTempDependencies = tempTableContainer.TempOnTempDependencies;
            _tempSqlQueriesList = tempTableContainer.TempSqlQueriesList;
            _alreadyAttachedTempTableQuery = new HashSet<string>();

            _assemblyVersion = typeof(EF6TempTableKitQueryInterceptor).Assembly.GetName().Version;
            _generatedByEf6TempTableKitStartMsg = $"/* Generated by EF6TempTableKit {_assemblyVersion} - START */";
            _generatedByEf6TempTableKitEndMsg = $"/* Generated by EF6TempTableKit {_assemblyVersion} - END*/";
        }

        public string BuildSqlForTempTables(string interceptedComandText)
        {
            var sqlStringBuilder = new StringBuilder();

            foreach (var tempSqlQuery in _tempSqlQueriesList)
            {
                var tempTableName = tempSqlQuery.Key;
                var tempTableNameWithBrackets = $"[{tempTableName}]";
                var isTempTableAlreadyAttached = _alreadyAttachedTempTableQuery.Contains(tempTableName);
                if ((!isTempTableAlreadyAttached || tempSqlQuery.Value.IsDataAppend) && interceptedComandText.Contains(tempTableNameWithBrackets))
                {
                    var hasTempTableDependencies = _tempOnTempDependencies.ContainsKey(tempTableName);
                    if (hasTempTableDependencies)
                    {
                        foreach (var tempTableDependency in _tempOnTempDependencies[tempTableName])
                        {
                            foreach (var query in _tempSqlQueriesList.Where(x => x.Key == tempTableDependency))
                            {
                                AppendIfNotAlreadyAttached(sqlStringBuilder, query.Value.SqlQuery, tempTableDependency, query.Value.IsDataAppend);
                            }
                        }
                    }
                    AppendIfNotAlreadyAttached(sqlStringBuilder, tempSqlQuery.Value.SqlQuery, tempTableName, tempSqlQuery.Value.IsDataAppend);
                }
            }

            return sqlStringBuilder.ToString();
        }

        private void AppendIfNotAlreadyAttached(StringBuilder sqlStringBuilder, string sqlStringToAppend, string tempTableName, bool isDataAppend)
        {
            var isTempTableAlreadyAttached = _alreadyAttachedTempTableQuery.Contains(tempTableName);
            if (!isTempTableAlreadyAttached || isDataAppend)
            {
                var selectCommandTextFormat = "\n{0}\n{1}\n{2}\n";
                sqlStringBuilder.AppendFormat(selectCommandTextFormat, _generatedByEf6TempTableKitStartMsg, sqlStringToAppend, _generatedByEf6TempTableKitEndMsg);
                _alreadyAttachedTempTableQuery.Add(tempTableName);
            }
        }
    }
}
