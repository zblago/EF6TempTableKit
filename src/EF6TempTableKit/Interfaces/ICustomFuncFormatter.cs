using System;

namespace EF6TempTableKit.Interfaces
{
    interface ICustomFuncFormatter<in T, out Y>
    {
        Func<T, Y> Formatter { get; }
    }
}
