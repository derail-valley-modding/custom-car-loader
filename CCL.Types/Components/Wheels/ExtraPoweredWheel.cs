using CCL.Types.Proxies.Wheels;
using UnityEngine;

namespace CCL.Types.Components.Wheels
{
    [AddComponentMenu("CCL/Components/Wheels/Extra Powered Wheel")]
    public class ExtraPoweredWheel : PoweredWheelProxy, ICustomSerialized
    {
        public class CoupledAxle
        {
            public Transform Transform = null!;
            public Vector3 Axis = Vector3.right;
            public float Multiplier = 1;
        }

        public CoupledAxle[] CoupledAxles = new CoupledAxle[0];

        private Transform[]? _transforms;
        private Vector3[]? _axis;
        private float[]? _multipliers;

        public void OnValidate()
        {
            int length = CoupledAxles.Length;
            _transforms = new Transform[length];
            _axis = new Vector3[length];
            _multipliers = new float[length];

            for (int i = 0; i < length; i++)
            {
                var item = CoupledAxles[i];
                _transforms[i] = item.Transform;
                _axis[i] = item.Axis;
                _multipliers[i] = item.Multiplier;
            }
        }

        public void AfterImport()
        {
            if (_transforms == null || _axis == null || _multipliers == null) return;

            int length = _transforms.Length;
            CoupledAxles = new CoupledAxle[length];

            for (int i = 0; i < length; i++)
            {
                CoupledAxles[i] = new CoupledAxle
                {
                    Transform = _transforms[i],
                    Axis = _axis[i],
                    Multiplier = _multipliers[i]
                };
            }
        }
    }
}
