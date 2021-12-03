using EF6TempTableKit.Interfaces;
using System;

namespace EF6TempTableKit.Test.CustomFormatter
{
    public class IntCustomFormatter : ICustomConverter<int, int>
    {
        public Func<int, int> Converter => (x) => x * 100;
    }
}
