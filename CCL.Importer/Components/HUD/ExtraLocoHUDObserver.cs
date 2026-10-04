using DV.HUD;
using UnityEngine;

namespace CCL.Importer.Components.HUD
{
    internal class ExtraLocoHUDObserver : MonoBehaviour
    {
        public LocoHUDObserver Observer = null!;

        public void AlerterValueChanged(float value)
        {
            var manager = ExtraInteriorControlsManager.GetOrAddToManager(Observer.controlsManager);

            if (manager.Alerter.HasValue)
            {
                ToggleControlIfValue(value, manager.Alerter.Value);
            }
        }

        public void CabOrientValueChanged(float value)
        {
            var manager = ExtraInteriorControlsManager.GetOrAddToManager(Observer.controlsManager);

            if (manager.CabOrient.HasValue)
            {
                MoveScrollable(manager.CabOrient.Value, (int)value);
            }
        }

        public void PantographValueChanged(float value)
        {
            var manager = ExtraInteriorControlsManager.GetOrAddToManager(Observer.controlsManager);

            if (manager.Pantograph.HasValue)
            {
                ToggleControlIfValue(value, manager.Pantograph.Value);
            }
        }

        private void ToggleControlIfValue(float value, InteriorControlsManager.ControlReference reference, int direction = 1)
        {
            if (value < 0.5f) return;

            MoveScrollable(reference, (reference.controlImplBase.Value < 0.5f) ? direction : (-direction));
        }

        private bool MoveScrollable(InteriorControlsManager.ControlReference reference, int notches)
        {
            if (reference.scrollable == null) return false;

            Observer.controlsManager.scrollableTimerUtil.MoveScrollable(reference.scrollable, notches);
            return true;
        }
    }
}
