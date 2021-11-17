namespace EF6TempTableKit.Extensions
{
    public static class StringExtensions
    {
        public static object WrapWithSingleQuotes(this string text) 
        {
            return $"'{text}'";
        }
    }
}
