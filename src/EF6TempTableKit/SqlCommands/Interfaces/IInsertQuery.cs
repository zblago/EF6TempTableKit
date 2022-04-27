using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    internal interface IInsertQuery
    {
        IExecute AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithTypes, string sqlSelectQuery);
        IExecute AddInsertQuery(IEnumerable<ITempTable> list);
    }
}
