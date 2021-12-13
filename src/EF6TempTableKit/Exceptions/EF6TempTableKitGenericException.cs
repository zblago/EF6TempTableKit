using System;

namespace EF6TempTableKit.Exceptions
{
    public class EF6TempTableKitGenericException : Exception
    {
        public EF6TempTableKitGenericException(string message) : base(message) { }
    }
}
