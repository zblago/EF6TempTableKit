using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface IInsertQuery
    {
        IExecute AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithTypes, string sqlSelectQuery);
        IExecute AddInsertQuery(IReadOnlyDictionary<string, string> fieldsWithTypes, IEnumerable<ITempTable> list);
        IExecute AddInsertQueryIfCreated(IReadOnlyDictionary<string, int> fieldsWithTypes, string sqlSelectQuery);
        IExecute AddInsertQueryIfCreated(IReadOnlyDictionary<string, string> fieldsWithTypes, IEnumerable<ITempTable> list);
    }
}
