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
        CabLights
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
}