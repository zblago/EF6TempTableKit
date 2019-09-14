using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface IDrop
    {
        ICreate DropIfExists();
        IInsertQuery CreateIfNotExists(IReadOnlyDictionary<string, string> fieldsWithTypes);
    }
}
