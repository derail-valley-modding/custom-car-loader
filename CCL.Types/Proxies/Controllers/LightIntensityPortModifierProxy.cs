using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class LightIntensityPortModifierProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortValueType.STATE, false)]
        public string lightIntensityModifierPortId;

        [Header("Intensity modifier port value mapping")]
        public float inMapMin;
        public float inMapMax = 1f;
        public float outMapMin;
        public float outMapMax = 1f;

        [Header("optional")]
        public CabLightsControllerProxy cabLightsController;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(lightIntensityModifierPortId), lightIntensityModifierPortId, DVPortValueType.STATE)
        };
    }
}
