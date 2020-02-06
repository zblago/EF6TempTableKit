using System;

namespace EF6TempTableKit.Test.Extensions
{
    internal static class StringExtensions
    {
        internal static String WrapWithSquareBrackets(this string s)
        {
            return "[" + s + "]";
        }
    }
}
