using CCL.Types.Components;
using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    public class HeadlightProxy : MonoBehaviour, ICustomSerialized
    {
        public VolumetricBeamControllerBaseProxy.VolumetricBeamData beamData = new VolumetricBeamControllerBaseProxy.VolumetricBeamData();
        public GameObject glare = null!;
        public Renderer headlightRenderer = null!;
        public Material emissionMaterialLit = null!;
        public Material emissionMaterialUnlit = null!;

        [SerializeField, HideInInspector]
        private VolumetricLightBeamProxy _beam = null!;
        [SerializeField, HideInInspector]
        private float _intensityOutsideMax;
        [SerializeField, HideInInspector]
        private float _intensityInsideMax;

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(CreateDefaultGlare), "Create Default Glare",
            "Will only create one if there is no object assigned to the glare yet")]
        [MethodButton(nameof(CreateDefaultBeam), "Create Default Beam",
            "Will only create one if there is no object assigned to the beam yet")]
        private bool _buttons;

        public void OnValidate()
        {
            if (glare != null)
            {
                glare.SetActive(false);
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

        private void CreateDefaultGlare()
        {
            if (glare != null) return;

            bool red = name.Contains("Red");

            glare = GameObject.CreatePrimitive(PrimitiveType.Quad);
            glare.name = "Glare";
            glare.transform.parent = transform;
            glare.transform.ResetLocal();
            glare.transform.localScale = red ? new Vector3(2, 2, 1) : new Vector3(4, 4, 1);

            foreach (var collider in glare.GetComponentsInChildren<Collider>(true))
            {
                DestroyImmediate(collider);
            }

            var renderer = glare.GetComponent<Renderer>();
            renderer.sharedMaterial = null;

            var grabber = glare.AddComponent<MaterialGrabberRenderer>();
            grabber.RenderersToAffect = new[] { renderer };
            grabber.Replacements = new[] { new MaterialGrabberRenderer.IndexToName
            {
                RendererIndex = 0,
                ReplacementName = red ? "TaillightsGlare" : "HeadlightsGlare"
            }};

            glare.SetActive(false);
        }

        private void CreateDefaultBeam()
        {
            if (beamData.beam != null) return;

            bool high = name.Contains("High");

            var beam = new GameObject("Beam").AddComponent<VolumetricLightBeamProxy>();
            beam.transform.parent = transform;
            beam.transform.ResetLocal();
            beam.transform.localEulerAngles = high ? new Vector3(1, 0, 0) : new Vector3(8, 0, 0);

            beamData.intensityOutsideMax = high ? 0.04f : 0.01f;
            beamData.intensityInsideMax = 0.1f;
            beamData.beam = beam;

            beam.gameObject.SetActive(false);
        }
    }
}
