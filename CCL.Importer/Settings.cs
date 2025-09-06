using System.Collections.Generic;
using UnityModManagerNet;

namespace CCL.Importer
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Prefer CCL In Job Generation", Tooltip = "Forces job generation to use CCL cars if available for that cargo")]
        public bool PreferCCLInJobs = false;

        [Draw("Use Verbose Logging", Tooltip = "Enable this if you experience bugs or are developing your own custom car")]
        public bool UseVerboseLogging = false;

        // Don't show these in the settings screen.
        public bool InfoDump = false;
        public HashSet<string> DisabledIds = new();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange() { }
    }
}
