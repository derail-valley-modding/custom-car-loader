using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyVanillaAudioSystem : MonoBehaviour
    {
        public VanillaAudioSystem AudioSystem;
    }

    public enum VanillaAudioSystem
    {
        FullDE2Audio,
        FullDE6Audio,
        FullDH4Audio,
        FullDM3Audio,
        FullS060Audio,
        FullS282Audio,
        FullMicroshunterAudio,

        SteamerCoalDump = 200,
        SteamerFire,
        SteamerFireboxWind,
        SteamerSand,
        SteamerSafetyRelease,
        SteamerBlowdown,
        SteamerChestAdmission,
        SteamerCylinderCrack,
        SteamerCylinderCock,
        SteamerChuff,
        SteamerInjector,
        SteamerValveGear,
        SteamerCrownSheet,
        SteamerAirPump,
        SteamerDynamo,
        SteamerBellRing,
        SteamerBellPump
    }
}
