using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Effects
{
    public class WheelSlideSparksController : MonoBehaviour
    {
        private const float TRACK_GAUGE_2 = 0.76f;

        public Transform[] sparkAnchors;

        [RenderMethodButtons]
        [MethodButton("CCL.Types.Effects.WheelSlideSparksController:AutoSetup", "Auto setup")]
        public bool buttonRender;

        [ContextMenu("Auto Setup")]
        public void AutoSetup()
        {
            Transform parent = transform.parent;

            if (parent == null)
            {
                return;
            }

            Transform bogie1 = parent.Find($"{CarPartNames.BOGIE_FRONT}/{CarPartNames.BOGIE_CAR}");

            if (bogie1 == null)
            {
                return;
            }

            Transform bogie2 = parent.Find($"{CarPartNames.BOGIE_REAR}/{CarPartNames.BOGIE_CAR}");

            if (bogie2 == null)
            {
                return;
            }

            AutoSetupInternal(bogie1, bogie2);
        }

        public void AutoSetupWithBogies(Transform bogie1, Transform bogie2)
        {
            if (bogie1 == null)
            {
                return;
            }

            if (bogie2 == null)
            {
                return;
            }

            AutoSetupInternal(bogie1, bogie2);
        }

        private void AutoSetupInternal(Transform bogie1, Transform bogie2)
        {
            List<Transform> sparks = new List<Transform>();
            sparks.AddRange(SetupBogie(bogie1));
            sparks.AddRange(SetupBogie(bogie2));
            sparkAnchors = sparks.ToArray();
        }

        private List<Transform> SetupBogie(Transform bogie)
        {
            Transform contacts = bogie.Find(CarPartNames.BOGIE_CONTACT_POINTS);
            List<Transform> points = new List<Transform>();

            // Contact point objects already exist.
            if (contacts != null)
            {
                foreach (Transform contact in contacts)
                {
                    points.Add(contact);
                }

                return points;
            }

            contacts = new GameObject(CarPartNames.BOGIE_CONTACT_POINTS).transform;
            contacts.parent = bogie;
            contacts.localPosition = Vector3.zero;

            int i = 0;

            foreach (Transform t in bogie)
            {
                if (!t.name.Equals(CarPartNames.AXLE))
                {
                    continue;
                }

                i++;

                Transform point = new GameObject($"{i}L").transform;
                point.parent = contacts.transform;
                point.localPosition = new Vector3(-TRACK_GAUGE_2, 0, t.localPosition.z);
                points.Add(point);

                point = new GameObject($"{i}R").transform;
                point.parent = contacts.transform;
                point.localPosition = new Vector3(TRACK_GAUGE_2, 0, t.localPosition.z);
                points.Add(point);
            }

            return points;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < sparkAnchors.Length; i++)
            {
                Gizmos.DrawSphere(sparkAnchors[i].position, 0.1f);
            }
        }
    }
}
