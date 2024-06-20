using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyVanillaParticleSystem : MonoBehaviour, IInstancedObject<GameObject>
    {
        public VanillaParticleSystem SystemToCopy;
        public bool ForcePlay = false;
        public bool AllowReplacing = true;
        public bool UseCustomSortingFudge = false;
        public float SortingFudge = 10;

        public GameObject? InstancedObject { get; set; }
        public bool CanReplace => AllowReplacing && InstancedObject != null;
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
