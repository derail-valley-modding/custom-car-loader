using CCL.Types.Proxies.Indicators;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Loco Indicator Reader Proxy")]
    public class LocoIndicatorReaderProxy : MonoBehaviour
    {
        public IndicatorProxy speed = null!;
        public IndicatorProxy tmTemp = null!;
        public IndicatorProxy oilTemp = null!;
        public IndicatorProxy amps = null!;
        public IndicatorProxy sand = null!;
        public IndicatorProxy oil = null!;
        public IndicatorProxy transmissionOil = null!;
        public IndicatorProxy fuel = null!;
        public IndicatorProxy battery = null!;
        public IndicatorProxy engineRpm = null!;
        public IndicatorProxy turbineRpmMeter = null!;
        public IndicatorProxy brakePipe = null!;
        public IndicatorProxy mainReservoir = null!;
        public IndicatorProxy brakeCylinder = null!;
        public IndicatorProxy voltage = null!;
        public IndicatorProxy availablePower = null!;
        public IndicatorProxy tenderCoalLevel = null!;
        public IndicatorProxy tenderWaterLevel = null!;
        public IndicatorProxy steam = null!;
        public IndicatorProxy chestPressure = null!;
        public IndicatorProxy locoWaterLevel = null!;
        public IndicatorProxy locoCoalLevel = null!;
        public IndicatorProxy fireTemperature = null!;
        public IndicatorProxy steamChest = null!;
        public IndicatorProxy waterInCylinder = null!;
        public IndicatorProxy cylinderTemperature = null!;
        public IndicatorProxy cylCocksPopped = null!;
    }
}
