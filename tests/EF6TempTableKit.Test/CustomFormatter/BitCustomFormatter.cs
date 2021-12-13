using EF6TempTableKit.Interfaces;
using System;
using System.Linq;

namespace EF6TempTableKit.Test.CustomFormatter
{
    public class BitCustomFormatter : ICustomFuncFormatter<byte[], string>
    {
        public Func<byte[], string> Formatter => (x) => $"0x{string.Join("", x.ToList().Select(item => item.ToString("X")))}";
    }
}