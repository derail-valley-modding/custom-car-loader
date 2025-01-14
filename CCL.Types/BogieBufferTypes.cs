using UnityEngine;

namespace CCL.Types
{
    public enum BogieType
    {
        Default         = 200,
        DE2             = 10,
        DE6             = 40,
        DH4             = 50,
        DM1U            = 35,
        S282            = 20,
        Handcar         = 700,
        Microshunter    = 70,
        UtilityFlatbed  = 220,
        Custom          = 10000
    }

    public enum BufferType
    {
        [Tooltip("Used by Boxcar")]
        Buffer02        = 400,
        [Tooltip("Used by Stock, DE2, DH4, DM3, DM1U")]
        Buffer03        = 350,
        [Tooltip("Used by Gondola, Utility Flatbed, S060")]
        Buffer04        = 550,
        [Tooltip("Used by DE6, DE6 Slug, Microshunter")]
        Buffer05        = 70,
        [Tooltip("Used by Military Boxcar, Refrigerator")]
        Buffer06        = 450,
        [Tooltip("Used by Caboose, Tank, Short Tank")]
        Buffer07        = 300,
        [Tooltip("Used by Passenger")]
        Buffer08        = 600,
        [Tooltip("Used by Autorack, Flatbed, Flatbed Military, Flatbed Stakes, Hopper, Closed Hopper, Nuclear Flask")]
        Buffer09        = 200,
        [Tooltip("Used by S282A (only front side)")]
        S282A           = 20,
        [Tooltip("Used by S282B (only rear side)")]
        S282B           = 21,
        [Tooltip("Use your own custom bogie")]
        Custom          = 10000
    }
}
