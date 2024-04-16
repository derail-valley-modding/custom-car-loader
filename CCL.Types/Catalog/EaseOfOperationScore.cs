using System;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class EaseOfOperationScore : ScoreList
    {        
        public CatalogScore LearningSimplicity = new CatalogScore();
        public CatalogScore BrakeOperation = new CatalogScore();
        public CatalogScore WarningSystems = new CatalogScore();
        public CatalogScore MisuseTolerance = new CatalogScore();
        public CatalogScore RemoteConnectivity = new CatalogScore();
        public CatalogScore ControlsLayout = new CatalogScore();
        public CatalogScore ItemStorage = new CatalogScore();
        public CatalogScore ReversingComfort = new CatalogScore();
        public CatalogScore LightingOperation = new CatalogScore();
        public CatalogScore SoundComfort = new CatalogScore();

        public override CatalogScore[] AllScores => new[]
        {
            LearningSimplicity,
            BrakeOperation,
            WarningSystems,
            MisuseTolerance,
            RemoteConnectivity,
            ControlsLayout,
            ItemStorage,
            ReversingComfort,
            LightingOperation,
            SoundComfort
        };
    }
}
