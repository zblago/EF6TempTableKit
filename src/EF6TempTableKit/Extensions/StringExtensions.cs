namespace EF6TempTableKit.Extensions
{
    public static class StringExtensions
    {
        public static object SurroundWithSingleQuotes(this object text) 
        {
            return $"'{text}'";
        }
    }
}
