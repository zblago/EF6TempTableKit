namespace EF6TempTableKit
{
    public sealed class Query
    {
        public string QueryString { get; set; }
        public bool ReuseExisting { get; set; }
        public string EntityFrameworkName { get; set; }
    }
}
