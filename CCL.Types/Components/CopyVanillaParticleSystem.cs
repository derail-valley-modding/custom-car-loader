using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyVanillaParticleSystem : MonoBehaviour, IInstancedGO
    {
        public VanillaParticleSystem SystemToCopy;
        public bool ForcePlay = false;
        public bool AllowReplacing = true;

        public GameObject? InstancedGO { get; set; }
        public bool CanReplace => AllowReplacing && InstancedGO != null;
    }

    public enum VanillaParticleSystem
    {
        DieselExhaustSmoke1,
        DieselExhaustSmoke2,
        DieselHighTempSmoke,
        DieselDamageSmoke,

        SteamerSteamSmoke,
        SteamerSteamSmokeThick,
        SteamerEmberClusters,
        SteamerEmberSparks,
        SteamerCylCockWaterDripParticles,
        SteamerExhaustSmallWispy,
        SteamerExhaustSmallWave,
        SteamerExhaustSmallLeak,
        SteamerExhaustWispy,
        SteamerExhaustWave,
        SteamerExhaustLeak,
        SteamerExhaustLargeWispy,
        SteamerExhaustLargeWave,
        SteamerExhaustLargeLeak,
        SteamerSteamLeaks,

        FireboxFire,
        FireboxSparks,
    }
}
