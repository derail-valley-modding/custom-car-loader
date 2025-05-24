using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class ShovelCoalPileProxy : MonoBehaviourWithVehicleDefaults, IS060Defaults, IS282Defaults
    {
        [Tooltip("Whether this coal pile supplies infinite coal")]
        public bool isInfinite;
        [Tooltip("Applies if infinite is true")]
        public float coalChunkMass;

        public void ApplyS060Defaults()
        {
            isInfinite = false;
            coalChunkMass = 9.0f;
        }

        public void ApplyS282Defaults()
        {
            isInfinite = false;
            coalChunkMass = 48.0f;
        }
    }
}
