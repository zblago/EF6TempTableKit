using SharDev.EFInterceptor.SqlCommands.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharDev.EFInterceptor.SqlCommands
{
    public sealed class SqlInsertCommandBuilder : IDrop, ICreate, IInsertQuery, IExecute
    {
        private string _tempTableName;

        public StringBuilder _ddlQueryBuilder;
        public StringBuilder _dqlQueryBuilder;
        public StringBuilder _dmlQueryBuilder;

        private SqlInsertCommandBuilder(string tempTableName)
        {
            _tempTableName = tempTableName;

            _ddlQueryBuilder = new StringBuilder();
            _dqlQueryBuilder = new StringBuilder();
            _dmlQueryBuilder = new StringBuilder();
    }

        public static IDrop Begin(string tableName) => new SqlInsertCommandBuilder(tableName);

        public ICreate DropIfExists()
        {
            _ddlQueryBuilder.AppendLine($"IF OBJECT_ID('tempdb..{_tempTableName}') IS NOT NULL");
            _ddlQueryBuilder.AppendLine("BEGIN");
            _ddlQueryBuilder.AppendLine($"\tDROP TABLE {_tempTableName}");
            _ddlQueryBuilder.AppendLine("END");
            _ddlQueryBuilder.AppendLine();

            return this;
        }

        public IInsertQuery Create(IReadOnlyDictionary<string, string> fieldsWithTypes)
        {
            _ddlQueryBuilder.AppendLine($"CREATE TABLE {_tempTableName}");
            _ddlQueryBuilder.AppendLine("(");

            ushort i = 0;
            int count = fieldsWithTypes.Count;
            bool isLastItem = false;

            foreach (var fieldWithType in fieldsWithTypes)
            {
                isLastItem = ++i == count ? true : false;

                var fieldName = fieldWithType.Key;
                var fieldValue = fieldWithType.Value;

                _ddlQueryBuilder.AppendLine($"\t{fieldName} {fieldValue}{(isLastItem ? "" : ",")}");
            }
            _ddlQueryBuilder.AppendLine(")");

            return this;
        }

        public IExecute AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery)
        {
            var fieldsWithPositionsSorted = fieldsWithPositions.OrderBy(f => f.Value);
            var isFirstColumnGreaterThanZero = fieldsWithPositionsSorted.First().Value > 0;

            var selectedColumns = string.Join(", ", fieldsWithPositionsSorted.Select(f => f.Key).ToArray());
            _dmlQueryBuilder.AppendLine();
            _dmlQueryBuilder.AppendLine($"INSERT INTO {_tempTableName}({ selectedColumns }) ");

            var selectedColumnsInTopSelectClause = $"{ selectedColumns }";
            var selectedColumnsInSubSelectClause = $"{ (isFirstColumnGreaterThanZero ? "TempColumn, " : " ") } { selectedColumns }";

            _dqlQueryBuilder.AppendLine($"SELECT { selectedColumnsInTopSelectClause } FROM");
            _dqlQueryBuilder.AppendLine($"({sqlSelectQuery}) AS alias{_tempTableName.Replace("#", "")} ({ selectedColumnsInSubSelectClause })");
            _dqlQueryBuilder.AppendLine();

            return this;
        }

        public string Execute()
        {
            return _ddlQueryBuilder.Append(_dmlQueryBuilder).Append(_dqlQueryBuilder).ToString();
        }
    }
}
