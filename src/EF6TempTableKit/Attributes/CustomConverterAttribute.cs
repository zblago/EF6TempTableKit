using EF6TempTableKit.Exceptions;
using EF6TempTableKit.Interfaces;
using System;
using System.Linq;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomConverterAttribute : Attribute
    {
        public Type Type { get; private set;}

        public CustomConverterAttribute(Type type)
        {
            if (!type.GetInterfaces().Any(x => x.GetGenericTypeDefinition() == typeof(ICustomConverter<,>)))
            {
                throw new EF6TempTableKitGenericException($"EF6TempTableKit: Only { nameof(ICustomConverter<object, object>) } is allowed.");
            }

            this.Type = type;
        }
    }
}
