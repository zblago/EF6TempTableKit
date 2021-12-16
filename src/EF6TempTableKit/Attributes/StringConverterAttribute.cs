using System;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringConverterAttribute : Attribute
    {
        public string Format { get; private set; }
        public IFormatProvider FormatProvider { get; private set; }

        public StringConverterAttribute(string format)
        {
            this.Format = format;
        }

        public StringConverterAttribute(string format, IFormatProvider formatProvider) : this(format)
        {
            this.FormatProvider = formatProvider;
        }
    }
}
