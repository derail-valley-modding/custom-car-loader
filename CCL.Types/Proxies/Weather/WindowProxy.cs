using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Window Proxy")]
    public class WindowProxy : MonoBehaviour, ISelfValidation
    {
        public bool simulate = true;
        public MeshRenderer[] visuals = new MeshRenderer[0];
        public WiperProxy[] wipers = new WiperProxy[0];
        public WindowProxy[] duplicates = new WindowProxy[0];
        public Transform[] windowEdges = new Transform[0];
        public Vector2 sizeInMeters;
        public bool useBakedUVs;
        public bool mirrorX;
        public bool mirrorY;
        public RenderTexture dropletRenderingTexture = null!;
        public Rigidbody rb = null!;

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(SetupDuplicates), "Setup Duplicates")]
        [MethodButton(nameof(CreateWindowEdges), "Create Edges")]
        private bool _buttonRender;

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.identity;
            if (windowEdges != null)
            {
                for (int i = 0; i < windowEdges.Length; i++)
                {
                    Transform transform = windowEdges[i];
                    Transform transform2 = windowEdges[(i + 1) % windowEdges.Length];
                    if ((bool)transform && (bool)transform2)
                    {
                        Gizmos.DrawLine(transform.position, transform2.position);
                    }
                }
            }
            Gizmos.matrix = GetWindowMatrix();
            Gizmos.DrawWireCube(new Vector3(0.5f, 0.5f, 0f), Vector3.one);
        }

        private Matrix4x4 GetWindowMatrix()
        {
            return Matrix4x4.TRS(
                transform.TransformPoint(
                    new Vector3((0f - sizeInMeters.x) * 0.5f * (!mirrorX ? 1 : -1),
                    (0f - sizeInMeters.y) * 0.5f * (!mirrorY ? 1 : -1), 0f)),
                transform.rotation * Quaternion.Euler(mirrorY ? 180 : 0, mirrorX ? 180 : 0, 0f),
                new Vector3(sizeInMeters.x, sizeInMeters.y, 0.1f));
        }

        private void SetupDuplicates()
        {
            duplicates = transform.parent.GetComponentsInChildren<WindowProxy>().Where(x => x != this).ToArray();
            simulate = true;

            foreach (var window in duplicates)
            {
                window.simulate = false;
                window.duplicates = new WindowProxy[0];
            }
        }

        private void CreateWindowEdges()
        {
            windowEdges = new Transform[4];
            Create(0, 0.5f * new Vector2(sizeInMeters.x, sizeInMeters.y));
            Create(1, 0.5f * new Vector2(sizeInMeters.x, 0f - sizeInMeters.y));
            Create(2, 0.5f * new Vector2(0f - sizeInMeters.x, 0f - sizeInMeters.y));
            Create(3, 0.5f * new Vector2(0f - sizeInMeters.x, sizeInMeters.y));

            void Create(int index, Vector2 pos)
            {
                var t = new GameObject("window edge").transform;
                t.parent = transform;
                t.localRotation = Quaternion.identity;
                t.localPosition = pos;
                windowEdges[index] = t;
            }
        }

        public SelfValidationResult Validate(out string message)
        {
            if (duplicates.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(duplicates), out message);
            }

            if (wipers.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(wipers), out message);
            }

            return this.Pass(out message);
        }
    }
}
