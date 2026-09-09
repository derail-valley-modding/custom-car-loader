using UnityEngine;

namespace CCL.Types.Catalog
{
    public enum VehicleType
    {
        Car,
        Locomotive,
        Tender,
        Slug,
        Booster,
        Draisine,
        Railcar,
        ControlCar,
        Support,
        Special
    }

    public enum VehicleRole
    {
        None = 0,
        [Tooltip("The vehicle can perform light shunting duties")]
        LightShunting = 10,
        [Tooltip("The vehicle can perform heavy shunting duties")]
        HeavyShunting,
        [Tooltip("The vehicle can perform light hauling duties")]
        LightHauling = 20,
        [Tooltip("The vehicle can perform heavy hauling duties")]
        HeavyHauling,
        [Tooltip("The vehicle supplies fuel/resources to another unit")]
        FuelSupply = 30,
        [Tooltip("The vehicle transports specialised crew onboard itself")]
        CrewTransport = 40,
        [Tooltip("The vehicle acts as support for a specialised crew")]
        CrewSupport,
        [Tooltip("The vehicle transports passengers onboard itself")]
        PassengerTransport = 50,
        [Tooltip("The vehicle transports freight onboard itself")]
        FreightTransport,
        [Tooltip("The vehicle transports utilities/tools onboard itself")]
        UtilityTransport,
        [Tooltip("The vehicle is used for track maintenance duties")]
        TrackMaintenance = 60
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
        [Tooltip("A dash in case the vehicle cannot do a role (i.e. Handcar cannot Haul or Shunt)")]
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

    public enum TonnageRating
    {
        Good,
        Medium,
        Bad
    }

    public enum CatalogColor
    {
        Green,
        Yellow,
        Orange,
        Red
    }
}
