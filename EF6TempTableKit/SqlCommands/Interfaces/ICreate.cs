using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface ICreate
    {
        IInsertQuery Create(IReadOnlyDictionary<string, string> fieldsWithTypes);
        IInsertQuery CreateIfNotExists(IReadOnlyDictionary<string, string> fieldsWithTypes);
    }
}
