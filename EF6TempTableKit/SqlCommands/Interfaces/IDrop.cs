namespace EF6TempTableKit.SqlCommands.Interfaces
{
    public interface IDrop
    {
        ICreate DropIfExists();
    }
}
