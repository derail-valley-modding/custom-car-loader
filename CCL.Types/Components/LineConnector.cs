using UnityEngine;

namespace CCL.Types.Components
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineConnector : MonoBehaviour
    {
        public Transform[] TransformsToConnect = new Transform[0];

        private LineRenderer _lineRenderer = null!;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void LateUpdate()
        {
            var positions = new Vector3[TransformsToConnect.Length];

            for (int i = 0; i < TransformsToConnect.Length; i++)
            {
                positions[i] = TransformsToConnect[i].position;
            }

            _lineRenderer.positionCount = TransformsToConnect.Length;
            _lineRenderer.SetPositions(positions);
        }

        private void OnValidate()
        {
            var renderer = GetComponent<LineRenderer>();
            var positions = new Vector3[TransformsToConnect.Length];

            for (int i = 0; i < TransformsToConnect.Length; i++)
            {
                positions[i] = TransformsToConnect[i] == null ? Vector3.zero : TransformsToConnect[i].position;
            }

            renderer.positionCount = TransformsToConnect.Length;
            renderer.SetPositions(positions);
        }
    }
}
