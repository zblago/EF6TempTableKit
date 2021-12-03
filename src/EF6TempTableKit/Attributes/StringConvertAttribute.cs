using System;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringConvertAttribute : Attribute
    {
        public string Format { get; private set; }
        public IFormatProvider FormatProvider { get; private set; }

        public StringConvertAttribute(string format)
        {
            this.Format = format;
        }

        public StringConvertAttribute(string format, IFormatProvider formatProvider) : this(format)
        {
            this.FormatProvider = formatProvider;
        }
    }
}
