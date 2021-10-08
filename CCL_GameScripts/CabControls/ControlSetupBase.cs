using System.Collections.Generic;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    [DisallowMultipleComponent]
    public abstract class ControlSetupBase : ComponentInitSpec
    {
        public abstract CabControlType ControlType { get; }

        public CabInputType InputBinding;
        public float MappedMinimum = 0;
        public float MappedMaximum = 1;
        public bool UseAbsoluteMappedValue = false;

        [ProxyField("colliderGameObjects")]
        public GameObject[] InteractionColliders;
    }

    public enum CabControlType
    {
        Lever,
        Button,
        Puller,
        Rotary,
        Toggle,
        Wheel
    }
}