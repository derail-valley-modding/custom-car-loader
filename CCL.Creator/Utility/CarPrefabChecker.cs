using CCL.Types;
using UnityEngine;

namespace CCL.Creator.Utility
{
    [AddComponentMenu("CCL Editor/Car Prefab Checker")]
    internal class CarPrefabChecker : MonoBehaviour, IEditorComponent
    {
        private static readonly Color s_bogies = Color.Lerp(Color.green, Color.white, 0.35f);
        private static readonly Color s_couplers = Color.Lerp(Color.blue, Color.white, 0.35f);
        private static readonly Color s_com = Color.Lerp(Color.red, Color.white, 0.35f);
        private static readonly Vector3 s_offset = new Vector3(0, 0, 15);

        public Transform? BogieF;
        public Transform? BogieR;
        public Transform? CouplerF;
        public Transform? CouplerR;
        public Transform? CoM;
        public int Gauge = 1435;

        private float HalfGauge => Gauge / 2000f;
        private float ActualGauge => Gauge / 1000f;

        public void OnValidate()
        {
            BogieF = transform.Find(CarPartNames.Bogies.FRONT);
            BogieR = transform.Find(CarPartNames.Bogies.REAR);

            CouplerF = transform.Find($"{CarPartNames.Couplers.RIG_FRONT}/{CarPartNames.Couplers.COUPLER_FRONT}");
            CouplerR = transform.Find($"{CarPartNames.Couplers.RIG_REAR}/{CarPartNames.Couplers.COUPLER_REAR}");

            CoM = transform.Find(CarPartNames.CENTER_OF_MASS);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;

            var gauge = HalfGauge;
            var track1 = new Vector3(gauge, 0, -20);
            var track2 = new Vector3(gauge, 0, 20);
            var arrow1 = new Vector3(-gauge, 0, -1.5f);
            var arrow2 = new Vector3(gauge, 0, -1.5f);
            var arrow3 = new Vector3(0, 0, 1.5f);

            Gizmos.DrawLine(-track1, -track2);
            Gizmos.DrawLine(track1, track2);
            Gizmos.DrawLine(arrow1, arrow3);
            Gizmos.DrawLine(arrow2, arrow3);
            Gizmos.DrawLine(arrow1 + s_offset, arrow3 + s_offset);
            Gizmos.DrawLine(arrow2 + s_offset, arrow3 + s_offset);
            Gizmos.DrawLine(arrow1 - s_offset, arrow3 - s_offset);
            Gizmos.DrawLine(arrow2 - s_offset, arrow3 - s_offset);

            Gizmos.color = s_bogies;

            if (BogieF != null)
            {
                DrawBogie(BogieF);
            }
            if (BogieR != null)
            {
                DrawBogie(BogieR);
            }

            Gizmos.color = s_couplers;

            if (CouplerF != null)
            {
                Gizmos.DrawWireCube(CouplerF.position, Vector3.one * 0.3f);
            }
            if (CouplerR != null)
            {
                Gizmos.DrawWireCube(CouplerR.position, Vector3.one * 0.3f);
            }

            Gizmos.color = s_com;

            if (CoM != null)
            {
                Gizmos.DrawWireSphere(CoM.position, 0.25f);
            }
        }

        private void DrawBogie(Transform bogie)
        {
            Gizmos.DrawLine(bogie.position, bogie.position + Vector3.up * 2);

            bogie = bogie.Find(CarPartNames.Bogies.BOGIE_CAR);

            if (bogie == null) return;

            foreach (Transform t in bogie)
            {
                if (t.name == CarPartNames.Bogies.AXLE)
                {
                    Gizmos.DrawLine(t.position - t.right * ActualGauge, t.position + t.right * ActualGauge);
                }
            }
        }
    }
}
