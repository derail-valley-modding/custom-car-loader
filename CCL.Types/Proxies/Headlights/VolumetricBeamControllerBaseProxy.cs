using System;
using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    [NotProxied]
    public class VolumetricBeamControllerBaseProxy : MonoBehaviour
    {
        [Serializable]
        public class VolumetricBeamData
        {
            public VolumetricLightBeamProxy beam;
            public float intensityOutsideMax;
            public float intensityInsideMax;
        }
    }
}
