using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface ICreate
    {
        IAddClusteredIndex Create(IReadOnlyDictionary<string, string> fieldsWithTypes);
    }
}
