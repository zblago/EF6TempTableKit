using EF6TempTableKit.Enums;

namespace EF6TempTableKit
{
    public sealed class Query
    {
        public string QueryString { get; set; }
        public bool ReuseExisting { get; set; }
        public QueryType QueryType { get; set; }
        public bool IsDataAppend { get; set; }
        public bool IsExecuted { get; set; }
    }
}
