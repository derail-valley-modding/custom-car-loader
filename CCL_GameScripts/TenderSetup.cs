using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts
{
    public class TenderSetup : ComponentInitSpec, ISimSetup
    {
        public override string TargetTypeName => "CustomTenderSimulation";
        public override bool DestroyAfterCreation => true;

        [HideInInspector]
        public LocoParamsType SimType => LocoParamsType.Tender;

        [ProxyField]
        public SimParamsSteam.SteamFuelType FuelType = SimParamsSteam.SteamFuelType.Coal;
        [ProxyField]
        public float WaterCapacityL = 45000;
        [ProxyField]
        public float FuelCapacity = 3000;
    }

    public class CoalPileSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "CustomCoalPile";
        public override bool DestroyAfterCreation => true;
    }
}
