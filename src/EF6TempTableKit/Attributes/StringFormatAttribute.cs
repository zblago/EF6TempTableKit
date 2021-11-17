using System;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringFormatAttribute : Attribute
    {
        public string Format { get; private set; }
        public IFormatProvider FormatProvider { get; private set; }

        public StringFormatAttribute(string format)
        {
            this.Format = format;
        }

        public StringFormatAttribute(string format, IFormatProvider formatProvider) : this(format)
        {
            this.FormatProvider = formatProvider;
        }
    }
}
