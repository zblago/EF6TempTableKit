namespace SharDev.EFInterceptor.SqlCommands.Interfaces
{
    public interface IDrop
    {
        ICreate DropIfExists();
    }
}
