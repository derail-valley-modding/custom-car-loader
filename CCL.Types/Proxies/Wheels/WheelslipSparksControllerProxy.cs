using CCL.Types.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class WheelslipSparksControllerProxy : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class WheelSparksDefinition
        {
            public PoweredWheelProxy poweredWheel = null!;
            public Transform sparksLeftAnchor = null!;
            public Transform sparksRightAnchor = null!;
        }

        private const float TRACK_GAUGE_2 = 0.76f;

        public WheelSparksDefinition[] wheelSparks = new WheelSparksDefinition[0];

        [SerializeField, HideInInspector]
        private PoweredWheelProxy[] _wheels = new PoweredWheelProxy[0];
        [SerializeField, HideInInspector]
        private Transform[] _lefts = new Transform[0];
        [SerializeField, HideInInspector]
        private Transform[] _rights = new Transform[0];

        [RenderMethodButtons]
        [MethodButton(nameof(AutoSetup), "Auto setup",
            "This will auto setup contact points on powered wheels")]
        public bool buttonRender;

        public void AutoSetup()
        {
            Transform root = transform.root;
            List<WheelSparksDefinition> sparks = new List<WheelSparksDefinition>();

            var slide = root.GetComponent<CustomWheelSlideSparks>();

            foreach (var item in root.GetComponentsInChildren<PoweredWheelProxy>())
            {
                sparks.Add(SetupWheel(item));
            }

            wheelSparks = sparks.ToArray();
        }

        private WheelSparksDefinition SetupWheel(PoweredWheelProxy wheel)
        {
            Transform t = wheel.transform;
            Transform contacts = t.Find(CarPartNames.Bogies.CONTACT_POINTS);

            if (contacts == null)
            {
                contacts = new GameObject(CarPartNames.Bogies.CONTACT_POINTS).transform;
                contacts.parent = t.parent;
                contacts.localPosition = Vector3.zero;
            }

            int i = Mathf.CeilToInt(contacts.childCount / 2);

            Transform l = new GameObject($"{i}L").transform;
            l.parent = contacts.transform;
            l.localPosition = new Vector3(-TRACK_GAUGE_2, 0, t.localPosition.z);

            Transform r = new GameObject($"{i}R").transform;
            r.parent = contacts.transform;
            r.localPosition = new Vector3(TRACK_GAUGE_2, 0, t.localPosition.z);

            return new WheelSparksDefinition
            {
                poweredWheel = wheel,
                sparksLeftAnchor = l,
                sparksRightAnchor = r
            };
        }

        public void OnValidate()
        {
            int length = wheelSparks.Length;
            _wheels = new PoweredWheelProxy[length];
            _lefts = new Transform[length];
            _rights = new Transform[length];

            for (int i = 0; i < length; i++)
            {
                _wheels[i] = wheelSparks[i].poweredWheel;
                _lefts[i] = wheelSparks[i].sparksLeftAnchor;
                _rights[i] = wheelSparks[i].sparksRightAnchor;
            }
        }

        public void AfterImport()
        {
            int length = Mathf.Min(_wheels.Length, _lefts.Length, _rights.Length);
            wheelSparks = new WheelSparksDefinition[length];

            for (int i = 0; i < length; i++)
            {
                wheelSparks[i] = new WheelSparksDefinition
                {
                    poweredWheel = _wheels[i],
                    sparksLeftAnchor = _lefts[i],
                    sparksRightAnchor = _rights[i]
                };
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (wheelSparks == null)
            {
                return;
            }

            Gizmos.color = Color.red;

            for (int i = 0; i < wheelSparks.Length; i++)
            {
                if (wheelSparks[i] != null &&
                    wheelSparks[i].poweredWheel != null)
                {
                    if (wheelSparks[i].sparksLeftAnchor != null)
                    {
                        Gizmos.DrawSphere(wheelSparks[i].sparksLeftAnchor.position, 0.1f);
                        Gizmos.DrawLine(wheelSparks[i].poweredWheel.transform.position, wheelSparks[i].sparksLeftAnchor.position);
                    }

                    if (wheelSparks[i].sparksRightAnchor != null)
                    {
                        Gizmos.DrawSphere(wheelSparks[i].sparksRightAnchor.position, 0.1f);
                        Gizmos.DrawLine(wheelSparks[i].poweredWheel.transform.position, wheelSparks[i].sparksRightAnchor.position);
                    }
                }
            }
        }

        private static void AutoSetup(WheelslipSparksControllerProxy proxy)
        {
            proxy.AutoSetup();
        }
    }
}
