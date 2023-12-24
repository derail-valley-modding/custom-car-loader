using CCL.Types.Json;
using System;
using UnityEngine;
using static CCL.Types.Proxies.Wheels.PoweredWheelRotationViaAnimationProxy;

namespace CCL.Types.Proxies.Wheels
{
    public class PoweredWheelRotationViaCodeProxy : PoweredWheelRotationBaseProxy
    {
        [Serializable]
        public struct TransformRotationConfig
        {
            public Transform transformToRotate;

            public Vector3 rotationAxis;

            public TransformRotationConfig(Transform transformToRotate)
            {
                this.transformToRotate = transformToRotate;
                rotationAxis = Vector3.right;
            }
        }

        public TransformRotationConfig[] additionalTransformsToRotate;

        [HideInInspector]
        public string? Json;

        public void OnValidate()
        {
            Json = JSONObject.ToJson(additionalTransformsToRotate);
        }

        public void AfterImport()
        {
            if (Json != null)
            {
                additionalTransformsToRotate = JSONObject.FromJson<TransformRotationConfig[]>(Json);
            }
            else
            {
                additionalTransformsToRotate = new TransformRotationConfig[0];
            }
        }
    }
}
