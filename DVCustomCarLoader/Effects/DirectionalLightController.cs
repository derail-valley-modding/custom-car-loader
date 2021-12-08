using System.Collections;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;
using CCL_GameScripts.CabControls;
using System;
using System.Collections.Generic;

namespace DVCustomCarLoader.Effects
{
    public class DirectionalLightController : MonoBehaviour, ILocoEventAcceptor, ILocoEventProvider
    {
        protected CustomLocoController locoController;
        public LocoEventManager EventManager { get; set; }

        public float MasterLightState = 0;
        public float Direction = 0;

        public SimEventType[] EventTypes => new[] { SimEventType.Headlights };

        public float ForwardIntensity { get; private set; }
        public float ReverseIntensity { get; private set; }

        protected void Start()
        {
            locoController = gameObject.GetComponent<CustomLocoController>();
            if (!locoController) enabled = false;
        }

        protected void Update()
        {
            Direction = locoController.reverser;

            float fwdLight = Mathf.Lerp(0, MasterLightState, Mathf.InverseLerp(0, 1, Direction));
            if (fwdLight != ForwardIntensity)
            {
                ForwardIntensity = fwdLight;
                EventManager.Dispatch(this, SimEventType.LightsForward, fwdLight);
            }

            float revLight = Mathf.Lerp(0, MasterLightState, Mathf.InverseLerp(0, -1, Direction));
            if (revLight != ReverseIntensity)
            {
                ReverseIntensity = revLight;
                EventManager.Dispatch(this, SimEventType.LightsReverse, revLight);
            }
        }

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is float headlights)
            {
                MasterLightState = headlights;
            }
        }
    }
}