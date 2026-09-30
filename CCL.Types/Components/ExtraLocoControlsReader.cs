using CCL.Types.Proxies.Controls;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Extra Loco Controls Reader")]
    public class ExtraLocoControlsReader : LocoControlsReaderProxy
    {
        [Header("Extra")]
        public GameObject? Alerter;
        public GameObject? CabOrient;
        public GameObject? Pantograph;
    }
}
