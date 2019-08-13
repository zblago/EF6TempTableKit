using System;
using System.Collections.Generic;

namespace SharDev.EFInterceptor.SqlUtility
{
    public class TempTableCreator
    {
        string _tempTableDefinition;

        public TempTableCreator CreateTempTable(List<Func<string>> fieldsMap, string tempTableName)
        {
            var tempTableCreate = "CREATE TABLE #" + tempTableName + " (";
            var fieldsDeclaration = "";
            fieldsMap.ForEach(m =>
            {
                fieldsDeclaration = fieldsDeclaration + m() + ",";
            });

            tempTableCreate = $"{tempTableCreate}{fieldsDeclaration.Remove(fieldsDeclaration.Length - 1, 1)})";
            _tempTableDefinition = $"{tempTableCreate};\n\nINSERT INTO #{tempTableName}";

            return this;
        }

        public string Insert(string query)
        {
            return _tempTableDefinition + "\n" + query + ";\n\n";
        }
    }
}
