using System;

namespace EF6TempTableKit.Test.Extensions
{
    internal static class StringExtensions
    {
        internal static String AddSquareBrackets(this string s)
        {
            return "[" + s + "]";
        }
    }
}
