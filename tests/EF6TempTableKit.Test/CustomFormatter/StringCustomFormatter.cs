using EF6TempTableKit.Interfaces;
using System;

namespace EF6TempTableKit.Test.CustomFormatter
{
    public class StringCustomFormatter : ICustomFuncFormatter<string, string>
    {
        public Func<string, string> Formatter => (x) => x + "kita";
    }
}