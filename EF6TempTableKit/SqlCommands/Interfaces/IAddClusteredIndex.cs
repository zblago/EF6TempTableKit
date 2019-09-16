namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface IAddClusteredIndex
    {
        IAddNonClusteredIndexes AddClusteredIndex(string[] fields);
    }
}