using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Explosion Activation On Signal Proxy")]
    public class ExplosionActivationOnSignalProxy : MonoBehaviour, IHasPortIdFields, ISelfValidation
    {
        public float bodyDamagePercentage;
        public float wheelsDamagePercentage;
        public float mechanicalPTDamagePercentage;
        public float electricalPTDamagePercentage;
        public ExplosionPrefab explosionPrefab;
        public float explosionParticlesDuration = 4f;
        public float windowsBreakingDelay = 0.5f;
        public Transform explosionAnchor = null!;
        [PortId(null, null, true)]
        public string explosionSignalPortId = string.Empty;
        public bool explodeTrainCar;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(explosionSignalPortId), explosionSignalPortId),
        };

        public SelfValidationResult Validate(out string message)
        {
            if (explosionAnchor == null)
            {
                return this.FailForNull(nameof(explosionAnchor), out message);
            }

            return this.Pass(out message);
        }
    }
}
