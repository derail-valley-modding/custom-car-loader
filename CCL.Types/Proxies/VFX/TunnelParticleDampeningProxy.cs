using UnityEngine;

namespace CCL.Types.Proxies.VFX
{
    public class TunnelParticleDampeningProxy : MonoBehaviour, IS060Defaults, IS282Defaults
    {
        public enum Bogie
        {
            Front,
            Rear
        }

        [Header("Particle systems will have their LimitVelocityOverLifetime.drag values overwritten.")]
        public GameObject[] systems;

        public Bogie bogie;

        public float dampening = 8f;
        public float minHeight = 4f;
        public float maxHeight = 16f;

        #region Defaults

        public void ApplyS060Defaults()
        {
            dampening = 4.0f;
            minHeight = 4.0f;
            maxHeight = 16.0f;
        }

        public void ApplyS282Defaults()
        {
            dampening = 3.0f;
            minHeight = 4.0f;
            maxHeight = 16.0f;
        }

        #endregion
    }
}
