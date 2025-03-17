using UnityEngine;

namespace CCL.Types.Components
{
    public class ManualOilingPoint : MonoBehaviour
    {
        public enum Model
        {
            CupOnly,
            CupAndPipe,
            CupWallAndPipe
        }

        private static Vector3 s_pos = new Vector3(0.00f, 0.02f, 0.00f);
        private static Vector3 s_size = new Vector3(0.05f, 0.05f, 0.05f);
        private static Vector3 s_posPlate = new Vector3(0.02f, -0.02f, 0.00f);
        private static Vector3 s_sizePlate = new Vector3(0.08f, 0.01f, 0.04f);
        private static Vector3 s_posWall = new Vector3(0.03f, 0.02f, 0.00f);
        private static Vector3 s_sizeWall = new Vector3(0.01f, 0.03f, 0.08f);

        public string SyncTag = "o0";
        public Model CupModel = Model.CupOnly;

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(s_pos, s_size);

            if (CupModel == Model.CupAndPipe)
            {
                Gizmos.DrawCube(s_posPlate, s_sizePlate);
            }
            else if (CupModel == Model.CupWallAndPipe)
            {
                Gizmos.DrawCube(s_posWall, s_sizeWall);
            }
        }
    }
}
