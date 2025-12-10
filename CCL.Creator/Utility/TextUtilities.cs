using System;
using UnityEngine;

namespace CCL.Creator.Utility
{
    internal class TextUtilities
    {
        // https://www.c-sharpcorner.com/article/fuzzy-search-in-c-sharp/
        public static int LevenshteinDistance(string s, string t)
        {
            // Special cases
            if (s == t) return 0;
            if (s.Length == 0) return t.Length;
            if (t.Length == 0) return s.Length;

            // Initialize the distance matrix
            int[,] distance = new int[s.Length + 1, t.Length + 1];
            for (int i = 0; i <= s.Length; i++) distance[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) distance[0, j] = j;

            // Calculate the distance
            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            // Return the distance
            return distance[s.Length, t.Length];
        }
    }
}
