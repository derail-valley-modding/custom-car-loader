using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyVanillaParticleSystem : MonoBehaviour
    {
        public VanillaParticleSystem SystemToCopy;
        public bool ForcePlay = false;
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
    }
}
