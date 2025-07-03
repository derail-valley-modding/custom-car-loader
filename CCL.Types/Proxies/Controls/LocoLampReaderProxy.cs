using CCL.Types.Proxies.Indicators;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Loco Lamp Reader Proxy")]
    public class LocoLampReaderProxy : MonoBehaviour
    {
        public LampControlProxy fuel = null!;
        public LampControlProxy battery = null!;
        public LampControlProxy oil = null!;
        public LampControlProxy automaticLubricator = null!;
        public LampControlProxy manualLubricator = null!;
        public LampControlProxy sandLow = null!;
        public LampControlProxy sandDeploying = null!;
        public LampControlProxy engineTemp = null!;
        public LampControlProxy oilTemp = null!;
        public LampControlProxy rpm = null!;
        public LampControlProxy turbineRpm = null!;
        public LampControlProxy voltage = null!;
        public LampControlProxy availablePower = null!;
        public LampControlProxy amp = null!;
        public LampControlProxy wheelSlip = null!;
        public LampControlProxy electronics = null!;
        public LampControlProxy cabLight = null!;
        public LampControlProxy tmOffline = null!;
        public LampControlProxy headlightsFront = null!;
        public LampControlProxy headlightsRear = null!;
        public LampControlProxy wipers = null!;
        public LampControlProxy brakePipe = null!;
        public LampControlProxy brakeCyl = null!;
        public LampControlProxy mainRes = null!;
    }
}
