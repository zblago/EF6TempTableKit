namespace EF6TempTableKit.Extensions
{
    public static class StringExtensions
    {
        public static object WrapWithSingleQuotes(this object text) 
        {
            return $"'{text}'";
        }
    }
}
