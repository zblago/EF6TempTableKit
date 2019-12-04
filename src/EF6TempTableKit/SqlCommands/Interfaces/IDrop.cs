using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface IDrop
    {
        ICreate DropIfExists();
        IAddClusteredIndex CreateIfNotExists(IReadOnlyDictionary<string, string> fieldsWithTypes);
    }
}
