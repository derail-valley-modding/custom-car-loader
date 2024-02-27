using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class ExplosionActivationOnSignalProxy : MonoBehaviour, IHasPortIdFields
    {
        public float bodyDamagePercentage;
        public float wheelsDamagePercentage;
        public float mechanicalPTDamagePercentage;
        public float electricalPTDamagePercentage;
        public ExplosionPrefab explosion;
        public float explosionParticlesDuration = 4f;
        public float windowsBreakingDelay = 0.5f;
        public Transform explosionAnchor;
        [PortId(null, null, true)]
        public string explosionSignalPortId;
        public bool explodeTrainCar;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(explosionSignalPortId), explosionSignalPortId, DVPortValueType.GENERIC),
        };
    }

    public enum ExplosionPrefab
    {
        ExplosionBoiler,
        ExplosionElectric,
        ExplosionHydraulic,
        ExplosionMechanical,
        ExplosionTMOverspeed,
    }
}
