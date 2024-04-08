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

        DE2Engine = 1000,
        DE2EnginePiston,
        DE2ElectricMotor,
        [InspectorName("DE2 TM Overspeed")]
        DE2TMOverspeed,
        DE2EngineIgnition,
        DE2Horn,

        SteamerCoalDump = 2000,
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

        BE2ElectricMotor = 3000,
        [InspectorName("BE2 TM Overspeed")]
        BE2TMOverspeed,
        [InspectorName("BE2 TM Controller")]
        BE2TMController,
        BE2Horn,

        CabFan = 5000,
    }
}
