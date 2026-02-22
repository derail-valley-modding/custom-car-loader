using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Rotator Port Reader Proxy")]
    public class RotatorPortReaderProxy : MonoBehaviour, IHasPortIdFields, ICustomSerialized
    {
        private const int GIZMO_SEGMENTS = 18;
        private static readonly Vector3 ROT_AXIS = Vector3.left;

        [Serializable]
        public class RotationData
        {
            public Transform transformToRotate = null!;
            public Vector3 rotationAxis = Vector3.forward;
            public float maxRps = 10f;
        }

        [PortId(null, null, false)]
        public string portId = string.Empty;
        public RotationData[] transformsToRotate = new RotationData[0];

        [Header("Editor Visualisation")]
        public float radius = 0.1f;

        [SerializeField, HideInInspector]
        private Transform[] _transforms = new Transform[0];
        [SerializeField, HideInInspector]
        private Vector3[] _axis = new Vector3[0];
        [SerializeField, HideInInspector]
        private float[] _rps = new float[0];

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId)
        };

        public void OnValidate()
        {
            _transforms = new Transform[transformsToRotate.Length];
            _axis = new Vector3[transformsToRotate.Length];
            _rps = new float[transformsToRotate.Length];

            for (int i = 0; i < transformsToRotate.Length; i++)
            {
                _transforms[i] = transformsToRotate[i].transformToRotate;
                _axis[i] = transformsToRotate[i].rotationAxis;
                _rps[i] = transformsToRotate[i].maxRps;
            }
        }

        public void AfterImport()
        {
            transformsToRotate = new RotationData[_transforms.Length];

            for (int i = 0; i < _transforms.Length; i++)
            {
                transformsToRotate[i] = new RotationData
                {
                    transformToRotate = _transforms[i],
                    rotationAxis = _axis[i],
                    maxRps = _rps[i]
                };
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            foreach (var item in transformsToRotate)
            {
                if (item == null || item.transformToRotate == null) continue;

                using (new GizmoUtil.MatrixScope(item.transformToRotate.localToWorldMatrix *
                    Matrix4x4.Rotate(Quaternion.FromToRotation(ROT_AXIS, item.rotationAxis))))
                {
                    Gizmos.DrawLine(ROT_AXIS * radius * 0.5f, -ROT_AXIS * radius * 0.5f);

                    var cross = Quaternion.AngleAxis(-45, ROT_AXIS) * (Vector3.up * radius);
                    var prev = cross;
                    var rot = Quaternion.AngleAxis(90.0f / GIZMO_SEGMENTS, ROT_AXIS);

                    Gizmos.DrawWireSphere(Vector3.zero, radius * 0.1f);

                    for (int i = 0; i < GIZMO_SEGMENTS; i++)
                    {
                        var normal = (float)i / GIZMO_SEGMENTS;

                        cross = rot * cross;

                        Gizmos.DrawLine(prev, cross);
                        Gizmos.DrawLine(-prev, -cross);

                        prev = cross;
                    }

                    // Draw a small arrow at the end of the arc.
                    var size = radius * 0.3f;
                    var up = Vector3.up * size;
                    var fwd = Vector3.forward * size;

                    Gizmos.DrawLine(prev, prev + up);
                    Gizmos.DrawLine(prev, prev + fwd);
                    Gizmos.DrawLine(-prev, -prev - up);
                    Gizmos.DrawLine(-prev, -prev - fwd);
                }
            }
        }
    }
}
