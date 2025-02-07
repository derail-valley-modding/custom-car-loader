using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    public class HeadlightProxy : MonoBehaviour, ICustomSerialized
    {
        public VolumetricBeamControllerBaseProxy.VolumetricBeamData beamData;
        public GameObject glare;
        public Renderer headlightRenderer;
        public Material emissionMaterialLit;
        public Material emissionMaterialUnlit;

        [SerializeField, HideInInspector]
        private VolumetricLightBeamProxy _beam;
        [SerializeField, HideInInspector]
        private float _intensityOutsideMax;
        [SerializeField, HideInInspector]
        private float _intensityInsideMax;

        public void OnValidate()
        {
            if (glare != null)
            {
                glare.gameObject.SetActive(false);
            }

            if (beamData == null) return;

            _beam = beamData.beam;
            _intensityOutsideMax = beamData.intensityOutsideMax;
            _intensityInsideMax = beamData.intensityInsideMax;

            if (_beam != null)
            {
                _beam.gameObject.SetActive(false);
            }
        }

        public void AfterImport()
        {
            beamData = new VolumetricBeamControllerBaseProxy.VolumetricBeamData()
            {
                beam = _beam,
                intensityOutsideMax = _intensityOutsideMax,
                intensityInsideMax = _intensityInsideMax
            };
        }
    }
}
