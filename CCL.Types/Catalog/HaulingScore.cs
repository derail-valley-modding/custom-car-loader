using System;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class HaulingScore : ScoreList
    {
        public CatalogScore LoadRating = new CatalogScore();
        public CatalogScore TopSpeed = new CatalogScore();
        public CatalogScore AdhesionControl = new CatalogScore();
        public CatalogScore HeatManagement = new CatalogScore();
        public CatalogScore Range = new CatalogScore();
        public CatalogScore CruiseDrivingEase = new CatalogScore();
        public CatalogScore HillclimbDrivingEase = new CatalogScore();
        public CatalogScore DynamicBraking = new CatalogScore();

        public override CatalogScore[] AllScores => new[]
        {
            LoadRating,
            TopSpeed,
            AdhesionControl,
            HeatManagement,
            Range,
            CruiseDrivingEase,
            HillclimbDrivingEase,
            DynamicBraking
        };
    }
}
