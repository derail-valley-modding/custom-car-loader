using UnityEngine;

namespace CCL.Types.Components.HUD
{
    [AddComponentMenu("CCL/Components/HUD/Extra Loco Controls Reader")]
    public class ExtraLocoControlsReader : MonoBehaviour
    {
        [Tooltip("Also used for the 2nd pantograph control if selected in the HUD")]
        public GameObject? Alerter;
        public GameObject? CabOrient;
        public GameObject? Pantograph;
    }
}
