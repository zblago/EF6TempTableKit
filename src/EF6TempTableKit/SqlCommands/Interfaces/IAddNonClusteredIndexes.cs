using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface IAddNonClusteredIndexes
    {
        IInsertQuery AddNonClusteredIndexes(IReadOnlyDictionary<string, string[]> indexesWithFields);
    }
}
