using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class BlowbackParticlePortReaderProxy : MonoBehaviour, IHasPortIdFields, IS060Defaults, IS282Defaults
    {
        public float blowbackAirflowThreshold = 1.5f;
        public ExplosionPrefab blowbackParticlesPrefab = ExplosionPrefab.Fire;
        public float particlesLifetime = 4f;
        public Transform spawnAnchor;
        [PortId(null, null, false)]
        public string forwardSpeedId;
        [PortId(null, null, false)]
        public string airflowId;
        [PortId(null, null, false)]
        public string fireOnId;
        [PortId(null, null, false)]
        public string fireboxDoorId;

        [RenderMethodButtons]
        [MethodButton(nameof(SetLayers), "Set Layers",
            "This will put this GameObject in the correct layer to be detected by the game")]
        public bool buttonRender;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(forwardSpeedId), forwardSpeedId),
            new PortIdField(this, nameof(airflowId), airflowId),
            new PortIdField(this, nameof(fireOnId), fireOnId),
            new PortIdField(this, nameof(fireboxDoorId), fireboxDoorId)
        };

        public void SetLayers()
        {
            gameObject.SetLayer(DVLayer.Train_Big_Collider);
        }

        #region Defaults

        public void ApplyS060Defaults()
        {
            blowbackAirflowThreshold = 1.5f;
            blowbackParticlesPrefab = ExplosionPrefab.Fire;
        }

        public void ApplyS282Defaults()
        {
            blowbackAirflowThreshold = 4.5f;
            blowbackParticlesPrefab = ExplosionPrefab.Fire;
        }

        #endregion
    }
}
