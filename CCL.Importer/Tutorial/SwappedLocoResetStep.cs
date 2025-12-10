using DV.HUD;
using DV.Simulation.Cars;
using DV.Tutorial.QT;
using UnityEngine;

namespace CCL.Importer.Tutorial
{
    internal class SwappedLocoResetStep : ALocoTutorialStep
    {
        private BaseControlsOverrider? _overrider;
        private InteriorControlsManager? _controls;

        public SwappedLocoResetStep(TrainCar loco)
            : base(loco, "", QTSemantic.Look, null, Vector3.zero, shouldRecheck: false)
        {
            if (loco != null)
            {
                _overrider = loco.GetComponentInChildren<BaseControlsOverrider>(true);
                _controls = loco.interior.GetComponentInChildren<InteriorControlsManager>();
            }
        }

        protected override void InternalMakeCurrent()
        {
            base.InternalMakeCurrent();

            if (_overrider != null && _controls != null)
            {
                SetToZero(InteriorControlsManager.ControlType.ElectricsFuse);
                SetToZero(InteriorControlsManager.ControlType.StarterFuse);
                SetToZero(InteriorControlsManager.ControlType.TractionMotorFuse);
                SetToZero(InteriorControlsManager.ControlType.CabLight);
                SetToZero(InteriorControlsManager.ControlType.StarterControl);
                SetToZero(InteriorControlsManager.ControlType.GearboxA);
                SetToZero(InteriorControlsManager.ControlType.GearboxB);

                // Apply neutral state after resetting controls in case the real neutral is different.
                _overrider.SetNeutralState();
                _overrider = null;

                void SetToZero(InteriorControlsManager.ControlType controlType)
                {
                    if (_controls.TryGetControl(controlType, out var control))
                    {
                        control.controlImplBase.SetValue(0f);
                    }
                }
            }
        }

        protected override bool InternalCheck()
        {
            return true;
        }
    }
}
