using System.Collections.Generic;

namespace EF6TempTableKit.Extensions
{
    public static class HashSetExtensionsExtensions
    {
        public static void AddIfNotExists(this HashSet<string> hs, string item)
        {
            if (!hs.Contains(item))
                hs.Add(item);
        }
    }
}
