using System;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class MaintenanceScore : ScoreList
    {
        public CatalogScore EnergyEconomy = new CatalogScore();
        public CatalogScore RepairEconomy = new CatalogScore();
        public CatalogScore SanderAdjustability = new CatalogScore();
        public CatalogScore WheelsLifetime = new CatalogScore();
        public CatalogScore PowertrainLifetime = new CatalogScore();
        public CatalogScore CollisionToughness = new CatalogScore();

        public override CatalogScore[] AllScores => new[]
        {
            EnergyEconomy,
            RepairEconomy,
            SanderAdjustability,
            WheelsLifetime,
            PowertrainLifetime,
            CollisionToughness
        };
    }
}
