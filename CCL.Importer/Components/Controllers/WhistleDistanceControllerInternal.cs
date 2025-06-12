using DV;
using DV.CabControls;
using DV.HUD;
using System.Collections;
using UnityEngine;

namespace CCL.Importer.Components.Controllers
{
    internal class WhistleDistanceControllerInternal : MonoBehaviour
    {
        public GameObject DummyControl = null!;
        public Transform RelativeTo = null!;
        public float DistanceTolerance = 0.03f;
        public float MaxStrengthDistance = 0.25f;

        private ControlImplBase _dummy = null!;
        private InteriorControlsManager? _icm;
        private float _smoothedValue;
        private float _velocity;
        private bool _vr;

        private IEnumerator Start()
        {
            _vr = VRManager.IsVREnabled();

            _dummy = DummyControl.GetComponent<ControlImplBase>();

            if (_dummy == null)
            {
                Debug.LogError("Can't find control for WhistleJointController, destroying self!");
                Destroy(this);
                yield break;
            }

            _icm = GetComponentInParent<InteriorControlsManager>();
            _dummy.InteractionAllowed = false;
        }

        private void OnDisable()
        {
            _dummy.SetValue(0, ControlImplBase.SetValueSource.Default);
            _smoothedValue = 0;
            _velocity = 0;
        }

        private void Update()
        {
            if (!TimeUtil.IsFlowing) return;

            if (!_vr && _icm != null && _icm.IsControlScrolledRecently(InteriorControlsManager.ControlType.Horn))
            {
                _smoothedValue = 1f;
                _velocity = 0f;
                return;
            }

            var value = Mathf.Max(0, Vector3.Distance(RelativeTo.position, transform.position) / MaxStrengthDistance - DistanceTolerance);
            _smoothedValue = Mathf.SmoothDamp(_smoothedValue, value, ref _velocity, 0.1f);

            if (value <= 0 && _dummy.Value > 0 && _smoothedValue < 0.01f)
            {
                _smoothedValue = 0;
                _velocity = 0f;
            }

            _dummy.SetValue(_smoothedValue, ControlImplBase.SetValueSource.Default);
        }
    }
}
