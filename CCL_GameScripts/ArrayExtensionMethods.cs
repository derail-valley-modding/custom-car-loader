using System;

namespace CCL_GameScripts
{
    internal static class ArrayExtensionMethods
    {
        internal static T[] Fill<T>(this T[] array, T item, int startIndex, int count)
        {
            int endIndex = Math.Min(startIndex + count, array.Length);
            for (int i = startIndex; i < endIndex; i++)
            {
                array[i] = item;
            }
            return array;
        }

        internal static T[] Fill<T>(this T[] array, T item)
        {
            return array.Fill(item, 0, array.Length);
        }
    }
}
