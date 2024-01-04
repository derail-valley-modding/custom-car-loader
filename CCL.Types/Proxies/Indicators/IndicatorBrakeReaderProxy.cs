using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public abstract class IndicatorBrakeReaderProxy : MonoBehaviour
    {
        [FuseId]
        public string fuseId;
    }

    public class IndicatorBrakeCylinderReaderProxy : IndicatorBrakeReaderProxy { }

    public class IndicatorBrakePipeReaderProxy : IndicatorBrakeReaderProxy { }

    public class IndicatorBrakeReservoirReaderProxy : IndicatorBrakeReaderProxy { }
}
