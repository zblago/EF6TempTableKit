using System;

namespace EF6TempTableKit.Test.CustomFormatter
{
    /// <summary>
    /// WrongIntCustomFormatter formatter doesn't implement ICustomFuncFormatter. Exception should be thrown.
    /// </summary>
    public class WrongIntCustomFormatter
    {
        public Func<int, int> Formatter => (x) => x * 100;
    }
}
