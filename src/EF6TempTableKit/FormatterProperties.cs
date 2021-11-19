using EF6TempTableKit.Attributes;
using System.Reflection;

namespace EF6TempTableKit
{
    internal class FormatterInfo 
    {
        public string Name { get; set; }
        public FormatterProperties[] FormatterProperties { get; set; }
    }

    internal class FormatterProperties
    {
        public StringFormatAttribute StringFormatAttribute { get; set; }
        public object Field { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }
}
