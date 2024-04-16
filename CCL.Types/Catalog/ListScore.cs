using System;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class CatalogScore
    {
        public ScoreType Type;
        [Range(1, 5)]
        public int Value;
    }
}
