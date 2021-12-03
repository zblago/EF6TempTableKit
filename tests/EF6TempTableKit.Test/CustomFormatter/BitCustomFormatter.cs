using EF6TempTableKit.Interfaces;
using System;
using System.Linq;

namespace EF6TempTableKit.Test.CustomFormatter
{
    public class BitCustomFormatter : ICustomConverter<byte[], string>
    {
        public Func<byte[], string> Converter => (x) => $"0x{string.Join("", x.ToList().Select(item => item.ToString("X")))}";
    }
}