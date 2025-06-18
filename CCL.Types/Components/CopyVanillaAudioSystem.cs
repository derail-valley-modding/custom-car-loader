using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyVanillaAudioSystem : MonoBehaviour, IInstancedObject<GameObject>, IHasPortIdFields
    {
        public VanillaAudioSystem AudioSystem;

        [PortId]
        public string PortId1 = string.Empty;
        [PortId]
        public string PortId2 = string.Empty;

        public Transform[] SourcePositions = new Transform[0];
        public AudioClip[] Clips = new AudioClip[0];

        public GameObject? InstancedObject { get; set; }
        public bool CanReplace => InstancedObject != null;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(PortId1), PortId1),
            new PortIdField(this, nameof(PortId2), PortId2)
        };

        public string[] Ports => new[]
        {
            PortId1,
            PortId2
        };
    }

    public enum VanillaAudioSystem
    {
        SandFlow = 0,
        [InspectorName("TM Overspeed")]
        TMOverspeed,
        CabFan,

        [InspectorName("")]
        SeparatorDE2 = 999,
        DE2Engine = 1000,
        DE2EnginePiston,
        DE2EngineIgnition,
        DE2ElectricMotor,
        DE2Horn,
        DE2Compressor,

        [InspectorName("")]
        SeparatorDE6 = 1049,
        DE6EngineIdle = 1050,
        DE6EngineThrottling,
        DE6EngineIgnition,
        DE6ElectricMotor,
        DE6DynamicBrakeBlower,
        DE6Horn,
        DE6Bell,
        DE6Compressor,

        [InspectorName("")]
        SeparatorDH4 = 1099,
        DH4Engine = 1100,
        DH4EnginePiston,
        DH4EngineIgnition,
        DH4FluidCoupler,
        DH4HydroDynamicBrake,
        DH4TransmissionEngaged,
        DH4ActiveCooler,
        DH4Horn,
        DH4Bell,
        DH4Compressor,

        [InspectorName("")]
        SeparatorDM3 = 1149,
        DM3Engine = 1150,
        DM3EnginePiston,
        DM3EngineIgnition,
        DM3JakeBrake,
        DM3TransmissionEngaged,
        DM3Horn,
        DM3Compressor,

        [InspectorName("")]
        SeparatorSteam = 2999,
        SteamerCoalDump = 3000,
        SteamerFire,
        SteamerFireboxWind,
        SteamerSafetyRelease,
        SteamerBlowdown,
        SteamerChestAdmission,
        SteamerCylinderCrack,
        SteamerCylinderCock,
        SteamerInjector,
        SteamerValveGear,
        SteamerValveGearDamaged,
        SteamerValveGearNoOil,
        SteamerCrownSheet,
        SteamerAirPump,
        SteamerDynamo,
        SteamerBellRing,
        SteamerBellPump,
        SteamerLubricator,
        SteamerPrimingCrank,

        S060Whistle = 3050,
        S282Whistle = 3100,

        [InspectorName("")]
        SeparatorBE2 = 4999,
        BE2ElectricMotor = 5000,
        [InspectorName("BE2 TM Controller")]
        BE2TMController,
        BE2Compressor,
        BE2Horn,
    }
}
