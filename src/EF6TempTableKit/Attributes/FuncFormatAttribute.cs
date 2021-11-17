using EF6TempTableKit.Exceptions;
using EF6TempTableKit.Interfaces;
using System;
using System.Linq;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FuncFormatAttribute : Attribute
    {
        public Type Type { get; private set;}

        public FuncFormatAttribute(Type type)
        {
            if (!type.GetInterfaces().Any(x => x == typeof(ICustomFuncFormatter<,>)))
            {
                throw new EF6TempTableKitGenericException($"EF6TempTableKit: Only { nameof(ICustomFuncFormatter<object, object>) } is allowed.");
            }

            this.Type = type;
        }
    }
}
