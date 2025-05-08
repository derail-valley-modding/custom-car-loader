namespace CCL.Types.Proxies.Controls
{
    public enum DVControlClass
    {
        Lever,
        Button,
        Rotary,
        Puller,
        ToggleSwitch,
        Wheel,
    }

    public enum OverridableControlType
    {
        None,
        Throttle = DVControlType.Throttle,
        TrainBrake = DVControlType.TrainBrake,
        Reverser = DVControlType.Reverser,
        IndBrake = DVControlType.IndBrake,
        Handbrake = DVControlType.Handbrake,
        Sander = DVControlType.Sander,
        //Horn = DVControlType.Horn,
        HeadlightsFront = DVControlType.HeadlightsFront,
        HeadlightsRear = DVControlType.HeadlightsRear,
        StarterControl = DVControlType.StarterControl,
        DynamicBrake = DVControlType.DynamicBrake,
        CabLight = DVControlType.CabLight,
        Wipers = DVControlType.Wipers,
        //FuelCutoff = DVControlType.FuelCutoff,
        IndCabLight = DVControlType.IndCabLight,
        Dynamo = DVControlType.Dynamo,
        AirPump = DVControlType.AirPump,
        TrainBrakeCutout = DVControlType.TrainBrakeCutout
    }

    public enum DVControlType
    {
        None,
        Throttle,
        TrainBrake,
        Reverser,
        IndBrake,
        Handbrake,
        Sander,
        Horn,
        HeadlightsFront,
        HeadlightsRear,
        StarterFuse,
        ElectricsFuse,
        TractionMotorFuse,
        StarterControl,
        DynamicBrake,
        CabLight,
        Wipers,
        FuelCutoff,
        ReleaseCyl,
        IndHeadlightsTypeFront,
        IndHeadlights1Front,
        IndHeadlights2Front,
        IndHeadlightsTypeRear,
        IndHeadlights1Rear,
        IndHeadlights2Rear,
        IndWipers1,
        IndWipers2,
        IndCabLight,
        IndDashLight,
        GearboxA,
        GearboxB,
        CylCock,
        Injector,
        Firedoor,
        Blower,
        Damper,
        Blowdown,
        CoalDump,
        Dynamo,
        AirPump,
        Lubricator,
        Bell,
        TrainBrakeCutout
    }
}
