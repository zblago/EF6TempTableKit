namespace EF6TempTableKit.Models
{
    internal sealed class TempTableInQuery
    {
        public string Name { get; set; }
        public bool IsAttachedInFinalQuery { get; set; } = false;
    }
}
