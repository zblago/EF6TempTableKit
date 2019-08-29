using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharDev.EFInterceptor.SqlUtility.SqlCommands
{
    public sealed class InsertQueryCreator : BaseCreator
    {
        private readonly string _tempTableName;

        public InsertQueryCreator(string tempTableName)
        {
            _tempTableName = tempTableName;
        }

        public InsertQueryCreator AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithPositions, string query)
        {
            var fieldsWithPositionsSorted = fieldsWithPositions.OrderBy(f => f.Value);
            var isFirstColumnGreaterThanZero = fieldsWithPositionsSorted.First().Value > 0;

            var selectedColumns = string.Join(", ", fieldsWithPositionsSorted.Select(f => f.Key).ToArray());
            _baseQueryBuilder.AppendLine();
            _baseQueryBuilder.AppendLine($"INSERT INTO {_tempTableName}({ selectedColumns }) ");

            var tempSelectBuilder = new StringBuilder();
            var selectedColumnsInTopSelectClause = $"{ selectedColumns }";
            var selectedColumnsInSubSelectClause = $"{ (isFirstColumnGreaterThanZero ? "TempColumn, " : " ") } { selectedColumns }";

            tempSelectBuilder.AppendLine($"SELECT { selectedColumnsInTopSelectClause } FROM");
            tempSelectBuilder.AppendLine($"({query}) AS alias{_tempTableName.Replace("#", "")} ({ selectedColumnsInSubSelectClause })");
            tempSelectBuilder.AppendLine();

            _baseQueryBuilder.Append(tempSelectBuilder);

            return this;
        }
    }
}
