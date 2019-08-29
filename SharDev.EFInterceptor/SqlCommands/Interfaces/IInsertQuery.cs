using System.Collections.Generic;

namespace SharDev.EFInterceptor.SqlCommands.Interfaces
{
    public interface IInsertQuery
    {
        IExecute AddInsertQuery(IReadOnlyDictionary<string, int> fieldsWithPositions, string sqlSelectQuery);
    }
}
