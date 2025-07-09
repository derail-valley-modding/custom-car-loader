using CCL.Types.Json;
using System.Collections.Generic;
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
    }
}
