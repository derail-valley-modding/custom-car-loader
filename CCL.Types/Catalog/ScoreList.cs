using System;
using System.Linq;

namespace CCL.Types.Catalog
{
    [Serializable]
    public abstract class ScoreList
    {
        public TotalScoreDisplay TotalScoreDisplay = TotalScoreDisplay.Average;

        public abstract CatalogScore[] AllScores { get; }
        public float Total => (float)AllScores.Select(x => x.Value).Average();
        public string FormattedTotal => ProperFormat(Total);

        private static string ProperFormat(float value)
        {
            var result = $"{value:F1}";

            if (result.EndsWith(".0"))
            {
                return $" {result.Substring(0, result.Length - 2)} ";
            }

            return result;
        }
    }
}
