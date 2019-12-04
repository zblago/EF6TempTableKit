using System.Text;

namespace EF6TempTableKit.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Prepend(this StringBuilder sb, string content)
        {
            return sb.Insert(0, content);
        }
    }
}