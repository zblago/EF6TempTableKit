using System.Collections.Generic;

namespace EF6TempTableKit.Extensions
{
    internal static class HashSetExtensionsExtensions
    {
        internal static void AddIfNotExists(this HashSet<string> hs, string item)
        {
            if (!hs.Contains(item))
                hs.Add(item);
        }
    }
}
