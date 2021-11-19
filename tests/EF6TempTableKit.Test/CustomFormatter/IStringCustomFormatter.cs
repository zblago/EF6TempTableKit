using System;

namespace EF6TempTableKit.Test.CustomFormatter
{
    public interface IStringCustomFormatter
    {
        Func<string, string> Formatter { get; }
    }
}