namespace CCL_GameScripts.CabControls
{
    public enum CabInputType
    {
        IndependentBrake,
        TrainBrake,
        Throttle,
        Reverser,
        Horn
    }

    public enum CabIndicatorType
    {
        BrakePipe,
        BrakeReservoir,
        EngineTemp,
        Fuel,
        Oil,
        Sand,
        Speed,
    }

    public enum SimEventType
    {
        Couplers,
        EngineDamage,
        EngineTemp,
        Fuel,
        Oil,
        Sand,
        Wheelslip,
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