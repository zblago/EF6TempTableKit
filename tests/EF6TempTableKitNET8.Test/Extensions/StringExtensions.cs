using System;

namespace EF6TempTableKitNET8.Test.Extensions
{
    internal static class StringExtensions
    {
        internal static String WrapWithSquareBrackets(this string s)
        {
            return "[" + s + "]";
        }
    }
}
