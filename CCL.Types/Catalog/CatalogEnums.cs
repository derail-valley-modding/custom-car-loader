using UnityEngine;

namespace CCL.Types.Catalog
{
    public enum VehicleType
    {
        Locomotive,
        Tender,
        Slug,
        Draisine,
        Car
    }

    public enum VehicleRole
    {
        None = 0,
        LightShunting = 10,
        HeavyShunting,
        LightHauling = 20,
        HeavyHauling,
        FuelSupply = 30,
        CrewTransport = 40,
        CrewSupport
    }

    public enum TechIcon
    {
        [Tooltip("Don't display technology")]
        None = 0,
        [Tooltip("Generic wrench icon for any technology that does not fit the other categories")]
        Generic,
        ClosedCab,
        OpenCab,
        CrewCompartment,
        CompressedAirBrakeSystem,
        DirectBrakeSystem,
        DynamicBrakeSystem,
        [Tooltip("Used for both Electric Power Supply and Electric Transmission")]
        ElectricPowerSupplyAndTransmission,
        ExternalControlInterface,
        HeatManagement,
        HydraulicTransmission,
        InternalCombustionEngine,
        MechanicalTransmission,
        PassengerCompartment,
        SpecializedEquipment,
        SteamEngine,
        [Tooltip("Depends on the type of this vehicle\n" +
            "Leave type empty to autofill based on vehicle type")]
        UnitEffect,
        CrewDelivery
    }

    public enum TotalScoreDisplay
    {
        [Tooltip("No score display")]
        None,
        [Tooltip("A score calculated from the values below")]
        Average,
        [Tooltip("A dash in case the vehicle cannot do a role (Handcar cannot Haul or Shunt)")]
        NotApplicable
    }

    public enum ScoreType
    {
        [Tooltip("An empty bar")]
        None,
        [Tooltip("An empty bar with the value '-'")]
        NotApplicable,
        [Tooltip("Normal coloured bar")]
        Score,
        [Tooltip("A coloured effect bar with a positive value")]
        PositiveEffect,
        [Tooltip("A coloured effect bar with a negative value")]
        NegativeEffect,
        [Tooltip("A shared effect bar with a positive value")]
        PositiveSharedEffect,
        [Tooltip("A shared effect bar with a negative value")]
        NegativeSharedEffect,
        [Tooltip("A bar with an effect that is turned on")]
        EffectOn,
        [Tooltip("A bar with an effect that is turned off")]
        EffectOff,
    }

    public enum CatalogColor
    {
        Green,
        Yellow,
        Orange,
        Red
    }
}
