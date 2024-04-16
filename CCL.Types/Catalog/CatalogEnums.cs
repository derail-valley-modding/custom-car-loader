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
        None
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
        [Tooltip("An empty bar with the value '0'")]
        Zero,
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
    }

    public enum CatalogColor
    {
        Green,
        Yellow,
        Orange,
        Red
    }
}
