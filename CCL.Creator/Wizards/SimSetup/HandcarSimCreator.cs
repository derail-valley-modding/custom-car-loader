using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Wheels;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class HandcarSimCreator : SimCreator
    {
        public HandcarSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "H1 (Handcar)" };

        public override IEnumerable<string> GetSimFeatures(int basisIndex)
        {
            yield return "Handlebar";
        }

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var drive = CreateSimComponent<HandcarDriveDefinitionProxy>("handcarDrive");
            var controller = CreateSibling<HandcarControllerProxy>(drive);
            controller.directionPortId = FullPortId(drive, "DIRECTION");
            controller.currentPositionPortId = FullPortId(drive, "CURRENT_POSITION");
            var throttle = CreateSibling<OverridableControlProxy>(drive);
            throttle.portId = FullPortId(drive, "BAR_EXT_IN");
            throttle.ControlType = OverridableControlType.Throttle;

            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");

            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);

            var poweredAxles = CreateSimComponent<ConfigurablePortDefinitionProxy>("poweredAxles");
            poweredAxles.value = 1;
            poweredAxles.port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM");

            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(poweredAxles, "NUM");
            wheelslip.engineBrakingActivePortId = FullPortId(drive, "ACTING_AGAINST");

            ConnectPorts(drive, "TORQUE_OUT", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            ConnectPortRef(drive, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(drive, "GEAR_RATIO", transmission, "GEAR_RATIO");

            ApplyMethodToAll<IH1Defaults>(s => s.ApplyH1Defaults());
        }
    }
}
