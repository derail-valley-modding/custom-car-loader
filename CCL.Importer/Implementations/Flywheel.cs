using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class Flywheel : SimComponent
    {
        private readonly float _rotationalInertia;
        private readonly float _viscousDampingFactor;
        private readonly float _maxRpm;
        private Port _rpm;
        private Port _rpmNormalised;
        private Port _rpmAbs;
        private Port _rpmAbsNormalised;
        private Port _torqueIn;
        private PortReference _loadTorque;

        public Flywheel(FlywheelDefinitionInternal def) : base(def.ID)
        {
            _rotationalInertia = def.RotationalInertia;
            _viscousDampingFactor = def.ViscousDampingFactor;
            _maxRpm = def.MaxRPM;

            _rpm = AddPort(def.RPM);
            _rpmNormalised = AddPort(def.RPMNormalised);
            _rpmAbs = AddPort(def.RPMAbsolute);
            _rpmAbsNormalised = AddPort(def.RPMAbsoluteNormalised);

            _torqueIn = AddPort(def.TorqueIn);
            _loadTorque = AddPortReference(def.LoadTorque);
        }

        public override void Tick(float delta)
        {
            var rpm = _rpm.Value;
            var damp = SimConsts.RPM_TO_RAD_PER_S * rpm * _viscousDampingFactor;
            var torque = _torqueIn.Value - damp + _loadTorque.Value;

            rpm += torque / _rotationalInertia * delta;

            // So it settles down rather than have very small values.
            if (rpm < 0.001 && rpm > -0.001)
            {
                rpm = 0;
            }

            var rpmNormalised = rpm / _maxRpm;

            // Absolute RPM is provided directly just in case.
            _rpm.Value = rpm;
            _rpmNormalised.Value = rpmNormalised;
            _rpmAbs.Value = Mathf.Abs(rpm);
            _rpmAbsNormalised.Value = Mathf.Abs(rpmNormalised);
        }
    }
}
