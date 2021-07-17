using System.Collections.Generic;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    [DisallowMultipleComponent]
    public abstract class ControlSetupBase : ComponentInitSpec
    {
        public abstract CabControlType ControlType { get; }

        [ProxyField("colliderGameObjects")]
        public GameObject[] InteractionColliders;
    }

    public enum CabControlType
    {
        Lever,
        Button
    }
}