using System;
using System.Linq.Expressions;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FuncFormatAttribute : Attribute
    {
        public FuncPointer Func { get; private set; }

        public string FieldType { get; private set; }

        public FuncFormatAttribute(string s)
        {
            //Func = func;
        }
    }


    public class FuncPointer 
    {
        public Func<string> Formatter { get; private set; }

        public FuncPointer() 
        {
            //Formatter = formatter;
        }
    }
}
