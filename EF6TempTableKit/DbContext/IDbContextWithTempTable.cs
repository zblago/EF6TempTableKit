namespace EF6TempTableKit.DbContext
{
    public interface IDbContextWithTempTable
    {
        TempTableContainer TempTableContainer { get; set; }
    }
}
