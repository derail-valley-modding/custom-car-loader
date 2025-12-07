using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Copiers/Copy Vanilla Particle System")]
    public class CopyVanillaParticleSystem : MonoBehaviour, IInstancedObject<GameObject>
    {
        public VanillaParticleSystem SystemToCopy;
        public bool ForcePlay = false;
        public bool AllowReplacing = true;
        public bool UseCustomSortingFudge = false;
        [EnableIf(nameof(UseCustomSortingFudge))]
        public float SortingFudge = 10;
        public bool UseCustomStartSize = false;
        [EnableIf(nameof(UseCustomStartSize))]
        public float StartSizeMultiplier = 1;
        [EnableIf(nameof(UseCustomStartSize))]
        public Vector3 StartSizeMultiplier3D = Vector3.one;
        public bool UseCustomShapeSize = false;
        [EnableIf(nameof(UseCustomShapeSize))]
        public Vector3 ShapeSizeMultiplier = Vector3.one;

        public GameObject? InstancedObject { get; set; }
        public bool CanReplace => AllowReplacing && InstancedObject != null;

        public void ApplyTo(ParticleSystem ps)
        {
            var main = ps.main;

            if (UseCustomStartSize)
            {
                main.startSizeMultiplier *= StartSizeMultiplier;
                main.startSizeXMultiplier *= StartSizeMultiplier3D.x;
                main.startSizeYMultiplier *= StartSizeMultiplier3D.y;
                main.startSizeZMultiplier *= StartSizeMultiplier3D.z;
            }

            var shape = ps.shape;

            if (UseCustomShapeSize)
            {
                var scale = shape.scale;
                scale.x *= ShapeSizeMultiplier.x;
                scale.y *= ShapeSizeMultiplier.y;
                scale.z *= ShapeSizeMultiplier.z;
                shape.scale = scale;
            }
        }
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
