using System;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NonClusteredIndexAttribute : Attribute
    {
        public string Name { get; private set; }
        public byte OrderNo { get; private set; }

        public NonClusteredIndexAttribute(string name, byte orderNo = 0)
        {
            this.Name = name;
            this.OrderNo = orderNo;
        }
    }
}
