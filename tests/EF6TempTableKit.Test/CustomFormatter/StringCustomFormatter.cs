using EF6TempTableKit.Interfaces;
using System;

namespace EF6TempTableKit.Test.CustomFormatter
{
    public class StringCustomFormatter : ICustomConverter<string, string>
    {
        public Func<string, string> Converter => (x) => "'" + x + "1" + "'";
    }
}