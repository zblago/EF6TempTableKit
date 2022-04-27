using System.Collections.Generic;

namespace EF6TempTableKit.SqlCommands.Interfaces
{
    internal interface IAddNonClusteredIndexes
    {
        IInsertQuery AddNonClusteredIndexes(IReadOnlyDictionary<string, string[]> indexesWithFields);
    }
}
