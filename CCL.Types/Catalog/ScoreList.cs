using System;
using System.Linq;

namespace CCL.Types.Catalog
{
    [Serializable]
    public abstract class ScoreList
    {
        public bool ShowTotalScore = true;

        public abstract CatalogScore[] AllScores { get; }
        public float Total => (float)AllScores.Select(x => x.Value).Average();
        public string FormattedTotal => $"{Total:F1}";
    }
}
