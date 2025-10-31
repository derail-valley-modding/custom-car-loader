using System;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Powered Wheel Rotation Via Code Proxy")]
    public class PoweredWheelRotationViaCodeProxy : PoweredWheelRotationBaseProxy, ICustomSerialized
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

        public TransformRotationConfig[] additionalTransformsToRotate = new TransformRotationConfig[0];

        [HideInInspector]
        [SerializeField]
        private Transform[] _transforms = new Transform[0];
        [HideInInspector]
        [SerializeField]
        private Vector3[] _axis = new Vector3[0];

        private PoweredWheelsManagerProxy? _cachedManager;

        public void OnValidate()
        {
            int length = additionalTransformsToRotate.Length;
            _transforms = new Transform[length];
            _axis = new Vector3[length];

            for (int i = 0; i < length; i++)
            {
                _transforms[i] = additionalTransformsToRotate[i].transformToRotate;
                _axis[i] = additionalTransformsToRotate[i].rotationAxis;
            }
        }

        public void AfterImport()
        {
            int length = Mathf.Min(_transforms.Length, _axis.Length);
            additionalTransformsToRotate = new TransformRotationConfig[length];

            for (int i = 0; i < length; i++)
            {
                additionalTransformsToRotate[i] = new TransformRotationConfig()
                {
                    transformToRotate = _transforms[i],
                    rotationAxis = _axis[i]
                };
            }
        }

        private void OnDrawGizmos()
        {
            if (_cachedManager == null)
            {
                _cachedManager = transform.root.GetComponentInChildren<PoweredWheelsManagerProxy>();
            }

            if (_cachedManager != null)
            {
                foreach (var item in _cachedManager.poweredWheels)
                {
                    if (item.wheelTransform != null)
                    {
                        DrawWheelGizmo(item.wheelTransform, item.localRotationAxis, wheelRadius, true);
                    }
                }
            }

            foreach (var item in additionalTransformsToRotate)
            {
                if (item.transformToRotate != null)
                {
                    DrawWheelGizmo(item.transformToRotate, item.rotationAxis, wheelRadius, true);
                }
            }
        }
    }
}
