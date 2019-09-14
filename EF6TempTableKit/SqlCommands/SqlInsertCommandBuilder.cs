using EF6TempTableKit.SqlCommands.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF6TempTableKit.SqlCommands
{
    public sealed class SqlInsertCommandBuilder : IDrop, ICreate, IInsertQuery, IExecute
    {
        private readonly string _tempTableName;

        public StringBuilder _queryBuilder;

        private SqlInsertCommandBuilder(string tempTableName)
        {
            _tempTableName = tempTableName;

            _queryBuilder = new StringBuilder();
        }

        public static IDrop Begin(string tableName) => new SqlInsertCommandBuilder(tableName);

        public ICreate DropIfExists()
        {
            _queryBuilder.AppendLine($"DECLARE @tempTableExist bit = 0");
            _queryBuilder.AppendLine($"IF OBJECT_ID('tempdb..{_tempTableName}') IS NOT NULL");
            _queryBuilder.AppendLine("BEGIN");
            _queryBuilder.AppendLine("\tSET @tempTableExist = 1");
            _queryBuilder.AppendLine($"\tDROP TABLE {_tempTableName}");
            _queryBuilder.AppendLine("END");
            _queryBuilder.AppendLine();

            return this;
        }

        public IInsertQuery Create(IReadOnlyDictionary<string, string> fieldsWithTypes)
        {
            CreateTable(fieldsWithTypes, 0);

            return this;
        }

        public IInsertQuery CreateIfNotExists(IReadOnlyDictionary<string, string> fieldsWithTypes)
        {
            _queryBuilder.AppendLine($"DECLARE @tempTable{_tempTableName}Created bit = 0");
            _queryBuilder.AppendLine($"IF OBJECT_ID('tempdb..{_tempTableName}') IS NULL");
            _queryBuilder.AppendLine("BEGIN");
            _queryBuilder.AppendLine($"\tSET @tempTable{_tempTableName}Created = 1");

            CreateTable(fieldsWithTypes, 1);

            _queryBuilder.AppendLine("END");

            return this;
        }

        public IExecute DropCreatedTable()
        {
            _queryBuilder.AppendLine($"\tDROP TABLE {_tempTableName}");

            return this;
        }

        public IExecute AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery)
        {
            BuildInsertQuery(fieldsWithPositions, sqlSelectQuery, 0);

            return this;
        }

        public IExecute AddInsertQueryIfCreated(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery)
        {
            _queryBuilder.AppendLine($"IF @tempTable{_tempTableName}Created = 1");
            _queryBuilder.AppendLine($"BEGIN");

            BuildInsertQuery(fieldsWithPositions, sqlSelectQuery, 2);

            _queryBuilder.AppendLine("END");

            return this;
        }

        public string Execute()
        {
            return _queryBuilder.ToString();
        }

        #region private

        private void CreateTable(IReadOnlyDictionary<string, string> fieldsWithTypes, byte tabsCount)
        {
            var repeatedTabs = new string('\t', tabsCount);

            _queryBuilder.AppendLine($"{repeatedTabs}CREATE TABLE {_tempTableName}");
            _queryBuilder.AppendLine($"{repeatedTabs}(");

            ushort i = 0;
            int count = fieldsWithTypes.Count;
            bool isLastItem = false;

            foreach (var fieldWithType in fieldsWithTypes)
            {
                isLastItem = ++i == count ? true : false;

                var fieldName = fieldWithType.Key;
                var fieldValue = fieldWithType.Value;

                _queryBuilder.AppendLine($"{repeatedTabs}\t{fieldName} {fieldValue}{(isLastItem ? "" : ",")}");
            }
            _queryBuilder.AppendLine($"{repeatedTabs})");
        }

        private void BuildInsertQuery(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery, byte tabsCount)
        {
            var repeatedTabs = new string('\t', tabsCount);

            var fieldsWithPositionsSorted = fieldsWithPositions.OrderBy(f => f.Value);
            var isFirstColumnGreaterThanZero = fieldsWithPositionsSorted.First().Value > 0;

            var selectedColumns = string.Join(", ", fieldsWithPositionsSorted.Select(f => f.Key).ToArray());
            _queryBuilder.AppendLine($"{repeatedTabs}INSERT INTO {_tempTableName}({ selectedColumns }) ");

            var selectedColumnsInTopSelectClause = $"{ selectedColumns }";
            var selectedColumnsInSubSelectClause = $"{ (isFirstColumnGreaterThanZero ? "TempColumn, " : "") }{ selectedColumns }";

            _queryBuilder.AppendLine($"{repeatedTabs}SELECT { selectedColumnsInTopSelectClause } FROM");
            _queryBuilder.AppendLine($"{repeatedTabs}({sqlSelectQuery}) AS alias{_tempTableName.Replace("#", "")} ({ selectedColumnsInSubSelectClause })");
        }

        #endregion
    }
}
