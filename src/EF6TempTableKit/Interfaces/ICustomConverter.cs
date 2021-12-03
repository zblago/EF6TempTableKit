using System;

namespace EF6TempTableKit.Interfaces
{
    interface ICustomConverter<T, Y>
    {
        Func<T, Y> Converter { get; }
    }
}
