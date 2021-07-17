using System.Collections.Generic;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    [DisallowMultipleComponent]
    public abstract class ControlSetupBase : MonoBehaviour
    {
        public abstract CabControlType ControlType { get; }
        public GameObject[] InteractionColliders;
    }

    public enum CabControlType
    {
        Lever,
        Button
    }
}