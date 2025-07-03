using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Light Intensity Port Modifier Proxy")]
    public class LightIntensityPortModifierProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortValueType.STATE, false)]
        public string lightIntensityModifierPortId = string.Empty;

        [Header("Intensity modifier port value mapping")]
        public float inMapMin;
        public float inMapMax = 1f;
        public float outMapMin;
        public float outMapMax = 1f;

        [Header("Optional")]
        public CabLightsControllerProxy cabLightsController = null!;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(lightIntensityModifierPortId), lightIntensityModifierPortId, DVPortValueType.STATE)
        };
    }
}
