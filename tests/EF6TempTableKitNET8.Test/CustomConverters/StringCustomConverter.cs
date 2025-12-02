using EF6TempTableKit.Interfaces;
using System;

namespace EF6TempTableKitNET8.Test.CustomConverters
{
    public class StringCustomConverter : ICustomConverter<string, string>
    {
        public Func<string, string> Converter => (x) => "'" + x + "1" + "'";
    }
}