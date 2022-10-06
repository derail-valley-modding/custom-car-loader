using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace DVCustomCarLoader
{
    public class CCLSettings : UnityModManager.ModSettings
    {
        public bool PreferCustomCargoContainersForJobs = false;
        public bool PreferCustomCarsForJobs = false;
        public bool VerboseMode = true;

        public SpawnerFeeBehavior LocoFeeBehavior = SpawnerFeeBehavior.DefaultNoTracking;

        public bool FeesForCCLLocos => 
            LocoFeeBehavior == SpawnerFeeBehavior.TrackCCLLocos ||
            LocoFeeBehavior == SpawnerFeeBehavior.TrackAllLocos;

        public bool FeesForAllLocos =>
            LocoFeeBehavior == SpawnerFeeBehavior.TrackAllLocos;

        public bool ForceShaderOverride = false;

        public void Draw(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical();

            PreferCustomCargoContainersForJobs = GUILayout.Toggle(PreferCustomCargoContainersForJobs, "Prefer custom cargo containers when generating jobs");
            PreferCustomCarsForJobs = GUILayout.Toggle(PreferCustomCarsForJobs, "Prefer custom cars to default cars when generating jobs");
            VerboseMode = GUILayout.Toggle(VerboseMode, "Verbose Logging");
            GUILayout.Space(2);

            GUILayout.Label("Career Manager fee behavior for player-spawned locos:");
            LocoFeeBehavior = (SpawnerFeeBehavior)GUILayout.SelectionGrid((int)LocoFeeBehavior, spawnerBehaviorDescriptions, 1, "toggle");
            GUILayout.Space(2);

            GUILayout.Label("Other Settings:");
            ForceShaderOverride = GUILayout.Toggle(ForceShaderOverride, "Force updated shader for older cars");
            GUILayout.Space(2);

            GUILayout.EndVertical();
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public enum SpawnerFeeBehavior
        {
            DefaultNoTracking,
            TrackCCLLocos,
            TrackAllLocos
        }

        private static readonly string[] spawnerBehaviorDescriptions = new[]
        {
            "Don't track spawned locos (default)",
            "Track player-spawned CCL locos",
            "Track all player-spawned locos"
        };
    }
}
