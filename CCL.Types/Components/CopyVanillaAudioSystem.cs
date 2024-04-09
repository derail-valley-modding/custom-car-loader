using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    public class CopyVanillaAudioSystem : MonoBehaviour, IInstancedObject<GameObject>
    {
        public VanillaAudioSystem AudioSystem;

        [PortId(null, null, false)]
        public string PortId1;
        [PortId(null, null, false)]
        public string PortId2;

        public GameObject? InstancedObject { get; set; }
        public bool CanReplace => true;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] {
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

        [InspectorName("")]
        SeparatorDE2 = 999,
        DE2Engine,
        DE2EnginePiston,
        DE2EngineIgnition,
        DE2ElectricMotor,
        DE2Horn,
        DE2Compressor,

        [InspectorName("")]
        SeparatorDE6 = 1099,
        DE6EngineIdle,
        DE6EngineThrottling,
        DE6EngineIgnition,
        DE6ElectricMotor,
        DE6DynamicBrakeBlower,
        DE6Horn,
        DE6Bell,
        DE6Compressor,

        [InspectorName("")]
        SeparatorDH4 = 1199,
        DH4Engine,
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
        SeparatorSteam = 1999,
        SteamerCoalDump,
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
        SteamerCrownSheet,
        SteamerAirPump,
        SteamerDynamo,
        SteamerBellRing,
        SteamerBellPump,

        [InspectorName("")]
        SeparatorBE2 = 2999,
        BE2ElectricMotor,
        [InspectorName("BE2 TM Controller")]
        BE2TMController,
        BE2Compressor,
        BE2Horn,

        [InspectorName("")]
        SeparatorMisc = 4999,
        CabFan,
    }
}
