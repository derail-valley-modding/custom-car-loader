using System;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class ShuntingScore : ScoreList
    {
        public CatalogScore Visibility = new CatalogScore();
        public CatalogScore Responsiveness = new CatalogScore();
        public CatalogScore ShuntAcceleration = new CatalogScore();
        public CatalogScore AccelerationDrivingEase = new CatalogScore();
        public CatalogScore SoloBraking = new CatalogScore();
        public CatalogScore CompressorStrength = new CatalogScore();
        public CatalogScore Compactness = new CatalogScore();
        public CatalogScore OutsideAccess = new CatalogScore();

        public override CatalogScore[] AllScores => new[]
        {
            Visibility,
            Responsiveness,
            ShuntAcceleration,
            AccelerationDrivingEase,
            SoloBraking,
            CompressorStrength,
            Compactness,
            OutsideAccess,
        };
    }
}
