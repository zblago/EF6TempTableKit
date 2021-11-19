using System;

namespace EF6TempTableKit.Interfaces
{
    interface ICustomFuncFormatter<T, Y>
    {
        Func<T, Y> Formatter { get; }
    }
}
