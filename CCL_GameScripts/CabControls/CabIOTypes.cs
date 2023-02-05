using System;

namespace CCL_GameScripts.CabControls
{
    public enum CabInputType
    {
        None,
        IndependentBrake,
        TrainBrake,
        Throttle,
        Reverser,
        Horn,
        
        Fuse,
        MainFuse,
        Starter,
        EngineStop,

        Sand,
        Fan,
        Headlights,
        CabLights,

        Blower,
        Damper,
        Injector,
        WaterDump,
        SteamDump,
        Whistle,
        FireDoor,
        Stoker,
        FireOut,

        SignalBoosterPower,

        Dynamo,
        Compressor,
        Bell,

        User1 = 1001, User2, User3, User4, User5, User6, User7, User8, User9, User10,
    }

    public enum SimEventType
    {
        None,
        Couplers,
        EngineDamage,
        EngineTemp,
        Fuel,
        Oil,
        Sand,
        Wheelslip,

        PowerOn,
        EngineOn,
        SandDeploy,
        Fan,

        BrakePipe,
        BrakeReservoir,
        IndependentPipe,
        IndependentReservoir,
        Speed,
        EngineRPM,
        EngineRPMGauge,

        Headlights,
        CabLights,
        LightsForward,
        LightsReverse,
        Amperage,

        FireTemp,
        WaterLevel,
        BoilerPressure,
        FireboxLevel,
        Cutoff,
        InjectorFlow,
        StokerFlow,

        WaterReserve,
        AccessoryPower,
        MUConnected,
        SignalBoosterPower,

        DynamoSpeed,
        CompressorSpeed,
    }

    public enum SimThresholdDirection
    {
        Above, Below
    }

    public enum SimAmount
    {
        Depleted = 0,
        Low = 1,
        Mid = 2,
        High = 3,
        Full = 4
    }

    [System.Serializable]
    public struct OutputBinding
    {
        public SimEventType SimEventType;
        public CabInputType CabInputType;

        public SimEventType[] EventTypes => 
            (SimEventType != SimEventType.None) 
                ? new[] { SimEventType }
                : null;

        public bool HasSimEvent => SimEventType != SimEventType.None;
        public bool HasControlBinding => CabInputType != CabInputType.None;

        public override string ToString() => $"({SimEventType}, {CabInputType})";

        public string GetName()
        {
            if ((SimEventType != SimEventType.None) && (CabInputType == CabInputType.None))
            {
                return Enum.GetName(typeof(SimEventType), SimEventType);
            }
            if ((CabInputType != CabInputType.None) && (SimEventType == SimEventType.None))
            {
                return Enum.GetName(typeof(CabInputType), CabInputType);
            }
            return $"{Enum.GetName(typeof(SimEventType), SimEventType)}, {Enum.GetName(typeof(CabInputType), CabInputType)}";
        }
    }

    public interface IBoundIndicator
    {
        OutputBinding Binding { get; }
    }
}