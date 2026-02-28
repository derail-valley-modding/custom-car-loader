using CCL.Types.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.VerletRope
{
    [AddComponentMenu("CCL/Proxies/Verlet Rope/Rope Behaviour Proxy")]
    public class RopeBehaviourProxy : MonoBehaviour, ICustomSerialized
    {
        public RopeMeshGeneratorProxy meshGenerator = null!;
        public RopeParams ropeParams = new RopeParams();
        public List<RopePin> pins = new List<RopePin>();

        [SerializeField, HideInInspector]
        private string? _params;
        [SerializeField, HideInInspector]
        private Vector3 _gravity;
        [SerializeField, HideInInspector]
        private Transform _forceTransform = null!;
        [SerializeField, HideInInspector]
        private float _floor;

        public void OnValidate()
        {
            //foreach (var pin in pins)
            //{
            //    if (pin == null) continue;

            //    pin.pointIndex = Mathf.Clamp(pin.pointIndex, 0, ropeParams.numPoints - 1);
            //}

            _params = JSONObject.ToJson(ropeParams);
            _gravity = ropeParams.gravity;
            _forceTransform = ropeParams.receiveForcesFrom;
            _floor = ropeParams.floorLevel;
        }

        public void AfterImport()
        {
            ropeParams = JSONObject.FromJson(_params, () => new RopeParams());
            ropeParams.gravity = _gravity;
            ropeParams.receiveForcesFrom = _forceTransform;
            ropeParams.floorLevel = _floor;
        }

        private void OnDrawGizmos()
        {
            if (pins.Count == 0) return;

            if (pins.Count == 1)
            {
                Gizmos.DrawLine(pins[0].transform.position, pins[0].transform.position + Vector3.forward * 0.2f);
                return;
            }

            var ordered = pins.OrderBy(x => x.pointIndex).ToList();

            for (int i = 1; i < ordered.Count; i++)
            {
                Gizmos.DrawLine(ordered[i - 1].transform.position, ordered[i].transform.position);
            }
        }
    }
}
