using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyVanillaAudioSystem : MonoBehaviour, IInstancedObject<GameObject>
    {
        public VanillaAudioSystem AudioSystem;

        public GameObject? InstancedObject { get; set; }
        public bool CanReplace => true;
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
