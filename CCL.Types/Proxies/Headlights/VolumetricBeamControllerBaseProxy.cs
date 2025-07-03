using System;
using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    [AddComponentMenu("CCL/Proxies/Headlights/Volumetric Beam Controller Proxy")]
    [NotProxied]
    public class VolumetricBeamControllerBaseProxy : MonoBehaviour
    {
        [Serializable]
        public class VolumetricBeamData
        {
            public VolumetricLightBeamProxy beam = null!;
            public float intensityOutsideMax;
            public float intensityInsideMax;
        }
    }
}
