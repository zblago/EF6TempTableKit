using EF6TempTableKit.Interfaces;
using System;

namespace EF6TempTableKit.Test.CustomFormatter
{
    public class IntCustomFormatter : ICustomFuncFormatter<int, int>
    {
        public Func<int, int> Formatter => (x) => x * 100;
    }
}
