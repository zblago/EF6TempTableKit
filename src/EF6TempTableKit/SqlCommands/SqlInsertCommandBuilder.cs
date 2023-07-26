using EF6TempTableKit.Attributes;
using EF6TempTableKit.Extensions;
using EF6TempTableKit.Interfaces;
using EF6TempTableKit.SqlCommands.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        
        public static IInsertQuery Continue(string tableName) => new SqlInsertCommandBuilder(tableName);

        public ICreate DropIfExists()
        {
            _queryBuilder.AppendLine($"DECLARE @{_tempTableExist} bit = IIF(OBJECT_ID('tempdb..{_tempTableName}') <> 0, 1, 0)");
            _queryBuilder.AppendLine($"IF @{_tempTableExist} = 1");
            _queryBuilder.AppendLine("BEGIN");
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

        public IExecute AddInsertQuery(IEnumerable<ITempTable> list) 
        {
            BuildInsertQuery(list, 0);

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

        public IExecute AddInsertQueryIfCreated(IEnumerable<ITempTable> list) 
        {
            _queryBuilder.AppendLine($"IF @{_tempTableExist} = 1");
            _queryBuilder.AppendLine($"BEGIN");

            BuildInsertQuery(list, 2);

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
            _queryBuilder.AppendLine($"SET @{_tempTableExist} = 1");
            _queryBuilder.AppendLine(Environment.NewLine);
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

        private void BuildInsertQuery(IEnumerable<ITempTable> list, byte tabsCount)
        {
            var repeatedTabs = new string('\t', tabsCount);

            var columns = list.First().GetType().GetProperties().Select((x, i) => new { Key = i, Value = x.Name });
            var customConverters = GetCustomConverters(list.First());

            var count = list.Count();
            var pageSize = 1000; //SQL Server limits INSERT to 1000 rows at once.
            var totalPageNo = Math.Ceiling((decimal)count / pageSize);

            for (var i = 0; i < totalPageNo; i++)
            {
                var pageItems = list.Skip(1000 * i).Take(1000);

                var selectedColumns = string.Join(", ", columns.Select(x => x.Value).ToArray());
                _queryBuilder.AppendLine($"{repeatedTabs}INSERT INTO {_tempTableName}({ selectedColumns }) ");

                _queryBuilder.AppendLine($@"VALUES {Environment.NewLine}{
                    string.Join(",", pageItems
                        .ToList()
                        .Select(x =>
                        $@"({
                            string.Join(",",
                            x.GetType().GetProperties()
                            .OrderBy(o => columns.Select(c => c.Value)
                            .ToList().IndexOf(o.Name))
                            .Select(property => property.GetSqlValue(x, customConverters)))
                            }){Environment.NewLine}")
                        .ToArray()) }");
            }
        }

        private static IDictionary<string, ConverterProperties[]> GetCustomConverters(ITempTable item)
        {
            var firstItemProperties = item.GetType().GetProperties();

            var defaultConverter = firstItemProperties
                .Where(x => !x.GetCustomAttributes(typeof(StringConverterAttribute), true).Any()
                    && !x.GetCustomAttributes(typeof(CustomConverterAttribute), true).Any())
                .Select(x => new ConverterInfo
                { 
                    Name = x.Name
                });

            var customStringConverters = firstItemProperties
                .Where(x => x.GetCustomAttributes(typeof(StringConverterAttribute), true).Any())
                .Select(x => new ConverterInfo
                {
                    Name = x.Name,
                    ConverterProperties = new ConverterProperties[] { 
                        new ConverterProperties 
                        { 
                            StringConvertAttribute = (StringConverterAttribute)x.GetCustomAttribute(typeof(StringConverterAttribute), true) 
                        }
                    }
                });

            var customFuncConverters = firstItemProperties
                .Where(x => x.GetCustomAttributes(typeof(CustomConverterAttribute), true).Any())
                .Select(x =>
                {
                    Type storeType = ((CustomConverterAttribute)x.GetCustomAttribute(typeof(CustomConverterAttribute), true))?.Type;
                    var instance = Activator.CreateInstance(storeType);
                    PropertyInfo info = instance.GetType().GetProperty(nameof(ICustomConverter<object, object>.Converter));
                    object field = info.GetValue(instance);
                    MethodInfo method = field.GetType().GetMethod(nameof(MethodBase.Invoke));

                    return new ConverterInfo
                    {
                        Name = x.Name,
                        ConverterProperties = new ConverterProperties[]
                        {
                            new ConverterProperties
                            {
                                Field = field,
                                MethodInfo = method
                            }
                        }
                    };
                });

            return defaultConverter
                .Union(customStringConverters)
                .Union(customFuncConverters)
                .GroupBy(x => x.Name)
                .Select(x => new ConverterInfo
                {
                    Name = x.Key,
                    ConverterProperties = x.Any(fp => fp.ConverterProperties != null && fp.ConverterProperties.Length > 0) 
                        ? x.SelectMany(xx => xx.ConverterProperties).ToArray()
                        : null
                })
                .ToDictionary(x => x.Name, x => x.ConverterProperties);
        }

        #endregion
    }
}