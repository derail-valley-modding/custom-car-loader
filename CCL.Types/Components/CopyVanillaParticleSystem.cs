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
        DieselExhaustSmoke1 = 0,
        DieselExhaustSmoke2,
        DieselHighTempSmoke,
        DieselDamageSmoke,

        SteamerSteamSmoke = 100,
        SteamerSteamSmokeThick,
        SteamerEmberClusters,
        SteamerEmberSparks,
        SteamerCylCockWaterDrip = 120,
        SteamerExhaustSmallWispy = 150,
        SteamerExhaustSmallWave,
        SteamerExhaustWispy,
        SteamerExhaustWave,
        SteamerExhaustLargeWispy,
        SteamerExhaustLargeWave,
        SteamerIndicatorWaterDrip = 180,
        SteamerSteamLeaks = 190,

        FireboxFire = 200,
        FireboxSparks,
    }
}
