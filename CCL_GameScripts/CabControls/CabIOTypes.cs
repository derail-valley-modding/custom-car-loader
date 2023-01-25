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

        public OutputBinding(SimEventType eventType, CabInputType cabInputType)
        {
            SimEventType = eventType;
            CabInputType = cabInputType;
        }

        public OutputBinding(SimEventType eventType) : this(eventType, CabInputType.None) { }
        public OutputBinding(CabInputType inputType) : this(SimEventType.None, inputType) { }

        public SimEventType[] EventTypes => 
            (SimEventType != SimEventType.None) 
                ? new[] { SimEventType }
                : null;

        public bool HasSimEvent => SimEventType != SimEventType.None;
        public bool HasControlBinding => CabInputType != CabInputType.None;

        public override string ToString() => $"({SimEventType}, {CabInputType})";
    }

    public interface IBoundIndicator
    {
        OutputBinding Binding { get; }
    }
}