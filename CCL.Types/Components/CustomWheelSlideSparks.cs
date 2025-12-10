using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Custom Wheel Slide Sparks")]
    public class CustomWheelSlideSparks : MonoBehaviour
    {
        private const float TRACK_GAUGE_2 = 0.76f;

        public Transform[] sparkAnchors = new Transform[0];

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(AutoSetup), "Auto setup",
            "This will auto setup contact points on the bogies. If you are only using default bogies, and no extra wheels, " +
            "you do not need to include this component at all.")]
        private bool _buttons;

        public void AutoSetup()
        {
            Transform parent = transform.parent;

            if (parent == null)
            {
                Debug.Log($"Cancelling auto setup (component is in the car root, " +
                    $"should be in a child named {CarPartNames.Particles.WHEEL_SPARKS} instead)");
                return;
            }

            AutoSetupWithBogies(
                parent.Find($"{CarPartNames.Bogies.FRONT}/{CarPartNames.Bogies.BOGIE_CAR}"),
                parent.Find($"{CarPartNames.Bogies.REAR}/{CarPartNames.Bogies.BOGIE_CAR}"));
        }

        public void AutoSetupWithBogies(Transform bogie1, Transform bogie2)
        {
            if (bogie1 == null)
            {
                Debug.Log("Cancelling auto setup (bogie1 is null)");
                return;
            }

            if (bogie2 == null)
            {
                Debug.Log("Cancelling auto setup (bogie2 is null)");
                return;
            }

            // Only use contact points of bogies, no need to worry about custom ones
            // during auto setup.
            List<Transform> sparks = new List<Transform>();
            sparks.AddRange(SetupBogie(bogie1));
            sparks.AddRange(SetupBogie(bogie2));
            sparkAnchors = sparks.ToArray();
        }

        private List<Transform> SetupBogie(Transform bogie)
        {
            Transform contacts = bogie.Find(CarPartNames.Bogies.CONTACT_POINTS);
            List<Transform> points = new List<Transform>();

            // Contact point objects already exist. This also works when grabbing
            // the default bogies.
            if (contacts != null)
            {
                foreach (Transform contact in contacts)
                {
                    points.Add(contact);
                }

                return points;
            }

            // If it doesn't exist, create a new contacts object in the bogie.
            contacts = new GameObject(CarPartNames.Bogies.CONTACT_POINTS).transform;
            contacts.parent = bogie;
            contacts.localPosition = Vector3.zero;
            // For sequential naming.
            int i = 0;

            foreach (Transform t in bogie)
            {
                if (!t.name.Equals(CarPartNames.Bogies.AXLE))
                {
                    continue;
                }

                i++;

                // Create 2 contact points on each axle, one on each side.
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
            if (sparkAnchors == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            for (int i = 0; i < sparkAnchors.Length; i++)
            {
                if (sparkAnchors[i] != null)
                {
                    Gizmos.DrawSphere(sparkAnchors[i].position, 0.1f);
                }
            }
        }
		
        private static void AutoSetupButton(CustomWheelSlideSparks controller)
        {
            controller.AutoSetup();
        }
    }
}
