using System;
using System.Linq;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class TechDescription
    {
        [Serializable]
        public class Entry
        {
            [TechDescription]
            public string Key = string.Empty;
        }

        public Entry[] Entries = new Entry[0];

        public string ToSequence() => string.Join(" ", Entries.Select(e => $"{{{e.Key}}}"));
    }
}
