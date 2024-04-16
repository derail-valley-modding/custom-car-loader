using UnityEngine;

namespace CCL.Types.Catalog
{
    public enum VehicleType
    {
        Locomotive
    }

    public enum VehicleRole
    {
        None = 0,
        ShuntingLight = 100,
        ShuntingHeavy,
        HaulingLight = 200,
        HaulingHeavy
    }

    public enum ScoreType
    {
        [Tooltip("No bar is displayed")]
        None,
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
}
