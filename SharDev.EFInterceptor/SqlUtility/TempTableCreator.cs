using System;
using System.Collections.Generic;

namespace SharDev.EFInterceptor.SqlUtility
{
    public class TempTableCreator
    {
        string _tempTableDefinition;

        public TempTableCreator CreateTempTable(List<Func<string>> fieldsMap, string tempTableName)
        {
            var dropTableCommand = DropTempTable(tempTableName);
            var tempTableCreate = $"{dropTableCommand}{Environment.NewLine}CREATE TABLE {tempTableName}(";
            var fieldsDeclaration = "";
            fieldsMap.ForEach(m =>
            {
                fieldsDeclaration = fieldsDeclaration + m() + ",";
            });

            tempTableCreate = $"{tempTableCreate}{fieldsDeclaration.Remove(fieldsDeclaration.Length - 1, 1)})";
            _tempTableDefinition = $"{tempTableCreate};\n\nINSERT INTO {tempTableName}";

            return this;
        }

        public string Insert(string query)
        {
            return _tempTableDefinition + "\n" + query + ";\n\n";
        }

        private string DropTempTable(string tempTableName)
        {
            return $"IF OBJECT_ID('tempdb..{tempTableName}') IS NOT NULL BEGIN DROP TABLE {tempTableName} END {Environment.NewLine}";
        }
    }
}
