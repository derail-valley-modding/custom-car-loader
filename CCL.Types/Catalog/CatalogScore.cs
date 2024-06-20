using System;
using UnityEngine;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class CatalogScore
    {
        public ScoreType ScoreType = ScoreType.Score;
        [Range(1, 5)]
        public int Value = 1;
    }
}
