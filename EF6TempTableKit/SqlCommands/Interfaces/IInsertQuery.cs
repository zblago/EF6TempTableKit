using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface IInsertQuery
    {
        IExecute AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery);
        IExecute AddInsertQueryIfCreated(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery);
    }
}
