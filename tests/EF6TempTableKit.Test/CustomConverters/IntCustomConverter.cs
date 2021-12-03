using EF6TempTableKit.Interfaces;
using System;

namespace EF6TempTableKit.Test.CustomConverters
{
    public class IntCustomConverter : ICustomConverter<int, int>
    {
        public Func<int, int> Converter => (x) => x * 100;
    }
}
