using System;

namespace EF6TempTableKit.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NonClusteredIndexAttribute : Attribute
    {
        public string Name { get; private set; }

        public NonClusteredIndexAttribute(string name)
        {
            this.Name = name;
        }
    }
}
