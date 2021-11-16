using System;

namespace EF6TempTableKit.Exceptions
{
    public class EF6TempTableGenericException : Exception
    {
        public EF6TempTableGenericException(string message) : base(message) { }
    }
}
