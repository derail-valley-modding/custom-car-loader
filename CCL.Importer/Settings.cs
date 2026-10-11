using DV;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace CCL.Importer
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Prefer CCL In Job Generation", Tooltip = "Forces job generation to use CCL cars if available for that cargo")]
        public bool PreferCCLInJobs = false;
        [Draw("Keep DE2 Spawns", Tooltip = "Keeps tracks that only spawn DE2s exclusive to DE2s")]
        public bool KeepDE2Spawns = false;

        [Draw("Use Verbose Logging", Tooltip = "Enable this if you experience bugs or are developing your own custom car")]
        public bool UseVerboseLogging = false;

        // Don't show these in the settings screen.
        public bool SpamWarningDisabled = false;
        public bool InfoDump = false;
        public HashSet<string> DisabledIds = new();
        public HashSet<string> DisabledLiveryIds = new();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void DrawGUI(UnityModManager.ModEntry modEntry)
        {
            this.Draw(modEntry);

            // This setting is already saved by the game, so we just provide a way to easily read and set it.
            // Has the benefit of not interfering with other mods that enable/disable it.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Error Ping Enabled");
            ErrorSoundLogHandler.SoundEnabled = GUILayout.Toggle(ErrorSoundLogHandler.SoundEnabled, string.Empty);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public void OnChange() { }
    }
}
