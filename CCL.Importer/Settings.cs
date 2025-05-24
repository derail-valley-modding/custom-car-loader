using UnityModManagerNet;

namespace CCL.Importer
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Use Verbose Logging", Tooltip = "Enable this if you experience bugs or are developing your own custom car")]
        public bool UseVerboseLogging = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange() { }
    }
}
