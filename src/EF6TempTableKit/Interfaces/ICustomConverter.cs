using System;

namespace EF6TempTableKit.Interfaces
{
    public interface ICustomConverter<T, Y>
    {
        Func<T, Y> Converter { get; }
    }
}
