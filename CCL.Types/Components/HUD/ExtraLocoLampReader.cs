using CCL.Types.Proxies.Indicators;
using UnityEngine;

namespace CCL.Types.Components.HUD
{
    [AddComponentMenu("CCL/Components/HUD/Extra Loco Lamp Reader")]
    public class ExtraLocoLampReader : MonoBehaviour
    {
        public LampControlProxy? Alerter;
        public LampControlProxy? Pantograph;
    }
}
