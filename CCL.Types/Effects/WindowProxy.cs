using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Effects
{
    public class WindowProxy : MonoBehaviour
    {
        public bool simulate;

        public MeshRenderer[] visuals;

        //public Wiper[] wipers;

        public WindowProxy[] duplicates;

        public Transform[] windowEdges;

        public Vector2 sizeInMeters;

        public bool useBakedUVs;

        public bool mirrorX;

        public bool mirrorY;

        //public RenderTexture dropletRenderingTexture;

        //public Rigidbody rb;

        [RenderMethodButtons]
        [MethodButton("CCL.Types.Effects.WindowProxy:SetupDuplicates", "Setup duplicates")]
        public bool buttonRender;

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
                    new Vector3((0f - sizeInMeters.x) * 0.5f * ((!mirrorX) ? 1 : (-1)),
                    (0f - sizeInMeters.y) * 0.5f * ((!mirrorY) ? 1 : (-1)), 0f)),
                transform.rotation * Quaternion.Euler(mirrorY ? 180 : 0, mirrorX ? 180 : 0, 0f),
                new Vector3(sizeInMeters.x, sizeInMeters.y, 0.1f));
        }

        private static void SetupDuplicates(WindowProxy proxy)
        {
            proxy.duplicates = proxy.transform.parent.GetComponentsInChildren<WindowProxy>().Where(x => x != proxy).ToArray();
            proxy.simulate = true;

            foreach (var window in proxy.duplicates)
            {
                window.simulate = false;
                window.duplicates = new WindowProxy[0];
            }
        }
    }
}
