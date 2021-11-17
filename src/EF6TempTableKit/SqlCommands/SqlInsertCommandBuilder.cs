using EF6TempTableKit.Attributes;
using EF6TempTableKit.Extensions;
using EF6TempTableKit.SqlCommands.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF6TempTableKit.SqlCommands
{
    internal sealed class SqlInsertCommandBuilder : IDrop, ICreate, IAddClusteredIndex, IAddNonClusteredIndexes, IInsertQuery, IExecute
    {
        private readonly string _tempTableName;
        private readonly string _tempTableExist;

        public StringBuilder _queryBuilder;

        internal SqlInsertCommandBuilder(string tempTableName)
        {
            _tempTableName = tempTableName;
            _tempTableExist = $"tempTable{ _tempTableName}Created";

            _queryBuilder = new StringBuilder();
        }

        public static IDrop Begin(string tableName) => new SqlInsertCommandBuilder(tableName);

        public ICreate DropIfExists()
        {
            _queryBuilder.AppendLine($"DECLARE @{_tempTableExist} bit = 0");
            _queryBuilder.AppendLine($"IF OBJECT_ID('tempdb..{_tempTableName}') IS NOT NULL");
            _queryBuilder.AppendLine("BEGIN");
            _queryBuilder.AppendLine($"\tSET @{_tempTableExist} = 1");
            _queryBuilder.AppendLine($"\tDROP TABLE {_tempTableName}");
            _queryBuilder.AppendLine("END");
            _queryBuilder.AppendLine();

            return this;
        }

        public IAddClusteredIndex Create(IReadOnlyDictionary<string, string> fieldsWithTypes)
        {
            CreateTable(fieldsWithTypes, 0);

            return this;
        }

        public IAddClusteredIndex CreateIfNotExists(IReadOnlyDictionary<string, string> fieldsWithTypes)
        {
            _queryBuilder.AppendLine($"DECLARE @{_tempTableExist} bit = 0");
            _queryBuilder.AppendLine($"IF OBJECT_ID('tempdb..{_tempTableName}') IS NULL");
            _queryBuilder.AppendLine("BEGIN");
            _queryBuilder.AppendLine($"\tSET @{_tempTableExist} = 1");

            CreateTable(fieldsWithTypes, 1);

            _queryBuilder.AppendLine("END");

            return this;
        }

        public IExecute DropCreatedTable()
        {
            _queryBuilder.AppendLine($"\tDROP TABLE {_tempTableName}");

            return this;
        }

        public IAddNonClusteredIndexes AddClusteredIndex(string[] fields)
        {
            if (fields.Length == 0) {
                return this;
            }

            var columnsList = string.Join(",", fields);

            _queryBuilder.AppendLine($"IF @{_tempTableExist} = 1");
            _queryBuilder.AppendLine($"BEGIN");

            var clusteredIndexString = $"\tCREATE CLUSTERED INDEX IX_{_tempTableName} ON {_tempTableName} ({columnsList});";
            _queryBuilder.AppendLine(clusteredIndexString);
            _queryBuilder.AppendLine("END");

            return this;
        }

        public IInsertQuery AddNonClusteredIndexes(IReadOnlyDictionary<string, string[]> indexesWithFields)
        {
            if (indexesWithFields.Count == 0)
            {
                return this;
            }

            _queryBuilder.AppendLine($"IF @{_tempTableExist} = 1");
            _queryBuilder.AppendLine($"BEGIN");

            foreach (var indexWithColumns in indexesWithFields)
            {
                var fields = indexWithColumns.Value;
                var indexName = $"{indexWithColumns.Key}";
                var columnsList = string.Join(",", fields);

                var clusteredIndexString = $"\tCREATE NONCLUSTERED INDEX IX_{indexName} ON {_tempTableName} ({columnsList});";
                _queryBuilder.AppendLine(clusteredIndexString);
            }

            _queryBuilder.AppendLine("END");
            _queryBuilder.AppendLine();

            return this;
        }

        public IExecute AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithTypes, string sqlSelectQuery)
        {
            BuildInsertQuery(fieldsWithTypes, sqlSelectQuery, 0);

            return this;
        }

        public IExecute AddInsertQuery(IReadOnlyDictionary<string, string> fieldsWithTypes, IEnumerable<ITempTable> list) 
        {
            BuildInsertQuery(fieldsWithTypes, list, 0);

            return this;
        }

        public IExecute AddInsertQueryIfCreated(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery)
        {
            _queryBuilder.AppendLine($"IF @{_tempTableExist} = 1");
            _queryBuilder.AppendLine($"BEGIN");

            BuildInsertQuery(fieldsWithPositions, sqlSelectQuery, 2);

            _queryBuilder.AppendLine("END");

            return this;
        }

        public IExecute AddInsertQueryIfCreated(IReadOnlyDictionary<string, string> fieldsWithTypes, IEnumerable<ITempTable> list) 
        {
            _queryBuilder.AppendLine($"IF @{_tempTableExist} = 1");
            _queryBuilder.AppendLine($"BEGIN");

            BuildInsertQuery(fieldsWithTypes, list, 2);

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

        private void BuildInsertQuery(IReadOnlyDictionary<string, int> fieldsWithTypes, string sqlSelectQuery, byte tabsCount)
        {
            var repeatedTabs = new string('\t', tabsCount);

            var fieldsWithPositionsSorted = fieldsWithTypes.OrderBy(f => f.Value);
            var isFirstColumnGreaterThanZero = fieldsWithPositionsSorted.First().Value > 0; 

            var selectedColumns = string.Join(", ", fieldsWithPositionsSorted.Select(f => f.Key).ToArray());
            _queryBuilder.AppendLine($"{repeatedTabs}INSERT INTO {_tempTableName}({ selectedColumns }) ");

            var selectedColumnsInTopSelectClause = $"{ selectedColumns }";
            var selectedColumnsInSubSelectClause = $"{ (isFirstColumnGreaterThanZero ? "TempColumn, " : "") }{ selectedColumns }";

            _queryBuilder.AppendLine($"{repeatedTabs}SELECT { selectedColumnsInTopSelectClause } FROM");
            _queryBuilder.AppendLine($"{repeatedTabs}({sqlSelectQuery}) AS alias{_tempTableName.Replace("#", "")} ({ selectedColumnsInSubSelectClause })");
        }

        private void BuildInsertQuery(IReadOnlyDictionary<string, string> fieldsWithTypes, IEnumerable<ITempTable> list, byte tabsCount)
        {
            var repeatedTabs = new string('\t', tabsCount);

            var columns = list.First().GetType().GetProperties().Select((x, i) => new { Key = i, Value = x.Name });
            var customStringFormatters = list.First().GetType()
                .GetProperties()
                .Select(x => new
                {
                    x.Name,
                    StringFormatAttribute = x.GetCustomAttributes(typeof(StringFormatAttribute), true).Union(x.GetCustomAttributes(typeof(FuncFormatAttribute), true).ToList())
                }).ToDictionary(x => x.Name, x => (Attribute[])x.StringFormatAttribute);

            var selectedColumns = string.Join(", ", columns.Select(x => x.Value).ToArray());
            _queryBuilder.AppendLine($"{repeatedTabs}INSERT INTO {_tempTableName}({ selectedColumns }) ");

            _queryBuilder.AppendLine($@"VALUES {Environment.NewLine}{
                string.Join(",", list
                    .ToList()
                    .Select(x =>
                    $@"({
                        string.Join(",",
                        x.GetType().GetProperties()
                        .OrderBy(o => columns.Select(c => c.Value)
                        .ToList().IndexOf(o.Name))
                        .Select(property => property.GetSqlValue(x, customStringFormatters)))
                        }){Environment.NewLine}")
                    .ToArray()) }");
        }

        #endregion
    }
}
