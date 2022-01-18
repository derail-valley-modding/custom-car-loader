using CCL_GameScripts.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL_GameScripts
{
    public class TenderSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "CustomTenderSimulation";
        public override bool DestroyAfterCreation => true;

        [ProxyField("_FuelType")]
        public SimParamsSteam.SteamFuelType FuelType = SimParamsSteam.SteamFuelType.Coal;
        [ProxyField]
        public float WaterCapacityL = 45000;
        [ProxyField]
        public float FuelCapacity = 3000;
    }
}
