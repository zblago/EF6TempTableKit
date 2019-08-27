using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharDev.EFInterceptor.SqlUtility
{
    public class TempTableCreator
    {
        private StringBuilder _tempTableDdl;

        public TempTableCreator()
        {
            _tempTableDdl = new StringBuilder();
        }

        public TempTableCreator Create(string tempTableName, IDictionary<string, string> fieldsWithTypes)
        {
            _tempTableDdl.AppendLine();
            _tempTableDdl.AppendLine($"CREATE TABLE {tempTableName}");
            _tempTableDdl.AppendLine("(");
            ushort i = 0;
            int count = fieldsWithTypes.Count;
            bool isLastItem = false;
            foreach (var fieldWithType in fieldsWithTypes)
            {
                isLastItem = ++i == count ? true : false;
                var fieldName = fieldWithType.Key;
                var fieldValue = fieldWithType.Value;
                _tempTableDdl.AppendLine($"\t{fieldName} {fieldValue}{(isLastItem ? "" : ",")}");
            }
            _tempTableDdl.AppendLine(")");

            return this;
        }

        public string AdInsertQuery(string tempTableName, IReadOnlyDictionary<string, int> fieldsWithPositions, string query)
        {
            var fieldsWithPositionsSorted = fieldsWithPositions.OrderBy(f => f.Value);
            var isFirstColumnGreaterThanZero = fieldsWithPositionsSorted.First().Value > 0;

            var selectedColumns = string.Join(", ", fieldsWithPositionsSorted.Select(f => f.Key).ToArray());
            _tempTableDdl.AppendLine();
            _tempTableDdl.AppendLine($"INSERT INTO {tempTableName}({ selectedColumns }) ");

            var tempSelectBuilder = new StringBuilder();
            var selectedColumnsInTopSelectClause = $"{ selectedColumns }";
            var selectedColumnsInSubSelectClause = $"{ (isFirstColumnGreaterThanZero ? "TempColumn, " : " ") } { selectedColumns }";

            tempSelectBuilder.AppendLine($"SELECT { selectedColumnsInTopSelectClause } FROM");
            tempSelectBuilder.AppendLine($"({query}) AS aliasTemp ({ selectedColumnsInSubSelectClause })");

            var finallQuery = tempSelectBuilder.AppendLine();

            return _tempTableDdl.Append(finallQuery).ToString();
        }

        public TempTableCreator DropIfExists(string tempTableName)
        {
            _tempTableDdl.AppendLine($"IF OBJECT_ID('tempdb..{tempTableName}') IS NOT NULL{Environment.NewLine}BEGIN{Environment.NewLine}\tDROP TABLE {tempTableName}{Environment.NewLine}END {Environment.NewLine}");

            return this;
        }
    }
}
