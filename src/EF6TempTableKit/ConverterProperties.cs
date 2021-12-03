using EF6TempTableKit.Attributes;
using System.Reflection;

namespace EF6TempTableKit
{
    internal class ConverterInfo 
    {
        public string Name { get; set; }
        public ConverterProperties[] ConverterProperties { get; set; }
    }

    internal class ConverterProperties
    {
        public StringConverterAttribute StringConvertAttribute { get; set; }
        public object Field { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }
}
