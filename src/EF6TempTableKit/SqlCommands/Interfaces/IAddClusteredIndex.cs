namespace EF6TempTableKit.SqlCommands.Interfaces
{
    internal interface IAddClusteredIndex
    {
        IAddNonClusteredIndexes AddClusteredIndex(string[] fields);
    }
}