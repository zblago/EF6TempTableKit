using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    internal interface IDrop
    {
        ICreate DropIfExists();
        IAddClusteredIndex CreateIfNotExists(IReadOnlyDictionary<string, string> fieldsWithTypes);
    }
}
