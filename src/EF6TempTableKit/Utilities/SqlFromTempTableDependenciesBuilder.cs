﻿using EF6TempTableKit.DbContext;
using EF6TempTableKit.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF6TempTableKit.Utilities
{
    /// <summary>
    /// Queries attached on context are reversed and stored into LIFO stack. We need that because the last query has more dependencies than first one.
    /// Every iteration has sql query (one that has create and load table sql query) that has dependencies on some other temp tables stored in flatten list (TempOnTempDependencies) sorted from those who have a lot dependencies to those with one or zero dependencies.
    /// </summary>
    internal sealed class SqlFromTempTableDependenciesBuilder
    {
        private readonly Version _assemblyVersion;
        private readonly string _generatedByEf6TempTableKitStartMsg;
        private readonly string _generatedByEf6TempTableKitEndMsg;
        private readonly TempTableContainer _tempTableContainer;

        internal SqlFromTempTableDependenciesBuilder(TempTableContainer tempTableContainer)
        {
            _tempTableContainer = tempTableContainer;

            _assemblyVersion = typeof(EF6TempTableKitQueryInterceptor).Assembly.GetName().Version;
            _generatedByEf6TempTableKitStartMsg = $"/* Generated by EF6TempTableKit {_assemblyVersion} - START */";
            _generatedByEf6TempTableKitEndMsg = $"/* Generated by EF6TempTableKit {_assemblyVersion} - END*/";
        }

        public string BuildSqlForTempTables(string interceptedComandText)
        {
            var alreadyAttachedTempTableQuery = new HashSet<string>();

            var sqlStringBuilder = new StringBuilder();
            var tempSqlQueriesEnumerator = _tempTableContainer.TempSqlQueriesList.Cast<DictionaryEntry>().Reverse().GetEnumerator();
            while (tempSqlQueriesEnumerator.MoveNext())
            {
                var tempTableName = (string)tempSqlQueriesEnumerator.Current.Key;
                var tempSqlQuery = (Query)tempSqlQueriesEnumerator.Current.Value;

                if (!alreadyAttachedTempTableQuery.Any(t => t == tempTableName) && interceptedComandText.Contains(tempTableName))
                {
                    AttachSqlStringAtTheBeginning(sqlStringBuilder, tempSqlQuery.QueryString);

                    var hasTempTableDependencies = _tempTableContainer?.TempOnTempDependencies.ContainsKey(tempTableName);
                    if (hasTempTableDependencies.Value)
                    {
                        foreach (var tempTable in _tempTableContainer?.TempOnTempDependencies[tempTableName])
                        {
                            var query = (Query)_tempTableContainer.TempSqlQueriesList[tempTable];
                            AttachSqlStringAtTheBeginning(sqlStringBuilder, query.QueryString);
                            alreadyAttachedTempTableQuery.Add(tempTable);
                        }
                    }
                }
            }

            return sqlStringBuilder.ToString();
        }

        private void AttachSqlStringAtTheBeginning(StringBuilder sqlStringBuilder, string sqlStringToAppend)
        {
            var selectCommandTextFormat = "\n{0}\n{1}\n{2}\n";

            sqlStringBuilder.Prepend(string.Format(selectCommandTextFormat, _generatedByEf6TempTableKitStartMsg, sqlStringToAppend, _generatedByEf6TempTableKitEndMsg));
        }
    }
}
