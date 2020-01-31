using System.Text;

namespace EF6TempTableKit.Extensions
{
    internal static class StringBuilderExtensions
    {
        internal static StringBuilder Prepend(this StringBuilder sb, string content)
        {
            return sb.Insert(0, content);
        }
    }
}