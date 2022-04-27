using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    internal interface ICreate
    {
        IAddClusteredIndex Create(IReadOnlyDictionary<string, string> fieldsWithTypes);
    }
}
