using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharDev.EFInterceptor.SqlUtility.SqlCommands
{
    public sealed class TempTableCreator : BaseCreator
    {
        private readonly string _tempTableName;

        public TempTableCreator(string tempTableName)
        {
            _tempTableName = tempTableName;
        }

        public TempTableCreator Create(IReadOnlyDictionary<string, string> fieldsWithTypes)
        {
            _baseQueryBuilder.AppendLine($"CREATE TABLE {_tempTableName}");
            _baseQueryBuilder.AppendLine("(");

            ushort i = 0;
            int count = fieldsWithTypes.Count;
            bool isLastItem = false;

            foreach (var fieldWithType in fieldsWithTypes)
            {
                isLastItem = ++i == count ? true : false;

                var fieldName = fieldWithType.Key;
                var fieldValue = fieldWithType.Value;

                _baseQueryBuilder.AppendLine($"\t{fieldName} {fieldValue}{(isLastItem ? "" : ",")}");
            }
            _baseQueryBuilder.AppendLine(")");

            return this;
        }

        public TempTableCreator DropIfExists()
        {
            _baseQueryBuilder.AppendLine($"IF OBJECT_ID('tempdb..{_tempTableName}') IS NOT NULL");
            _baseQueryBuilder.AppendLine("BEGIN");
            _baseQueryBuilder.AppendLine($"\tDROP TABLE {_tempTableName}");
            _baseQueryBuilder.AppendLine("END");
            _baseQueryBuilder.AppendLine();

            return this;
        }
    }
}
