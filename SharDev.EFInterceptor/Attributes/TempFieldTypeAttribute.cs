using System;

namespace SharDev.EFInterceptor.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TempFieldTypeAttribute : Attribute
    {
        public string FieldType { get; private set; }

        public TempFieldTypeAttribute(string fieldType)
        {
            this.FieldType = fieldType;
        }
    }
}
