using DV.CabControls;
using DV.HUD;
using DV.Localization;
using DV.UI.LocoHUD;
using DV.UIFramework;
using UnityEngine;

namespace CCL.Importer.Components.HUD
{
    internal class ExtraLocoHUDProvider : MonoBehaviour
    {
        public LocoHUDProvider Provider = null!;
        public ControlImplBase? Alerter;
        public ControlImplBase? CabOrient;
        public ControlImplBase? Pantograph;

        public void RemoveReferences()
        {
            Alerter = null;
            CabOrient = null;
            Pantograph = null;
        }

        // Controls.
        public void AlerterUpdated(ValueChangedEventArgs value)
        {
            if (Provider.locoControls.mechanical.alerter)
            {
                Provider.locoControls.mechanical.alerter.SetVisualLevel(value.newValue);
                SetControlNameFromObject(Provider.locoControls.mechanical.alerter, Alerter);
            }
        }

        public void CabOrientUpdated(ValueChangedEventArgs value)
        {
            if (Provider.locoControls.mechanical.cabOrient)
            {
                Provider.locoControls.mechanical.cabOrient.SetVisualLevel(value.newValue);
                SetControlNameFromObject(Provider.locoControls.mechanical.cabOrient, CabOrient);
            }
        }

        public void PantographUpdated(ValueChangedEventArgs value)
        {
            if (Provider.locoControls.mechanical.pantograph)
            {
                Provider.locoControls.mechanical.pantograph.SetVisualLevel(value.newValue);
                SetControlNameFromObject(Provider.locoControls.mechanical.pantograph, Pantograph);
            }
        }

        private void SetControlNameFromObject(LocoHUDControlBase control, ControlImplBase? impl)
        {
            if (impl == null)
            {
                control.SetTextUnit(string.Empty);
                control.SetTextValue(string.Empty);
                return;
            }

            (string value, string unit) = impl.GetCurrentPositionName();
            control.SetTextUnit(unit);
            control.SetTextValue(value.ToString(LocalizationAPI.CC));
        }

        // Lamps.
        public void AlerterAlarmUpdated(float value)
        {
            if (Provider.locoControls.mechanical.alerter)
            {
                Provider.locoControls.mechanical.alerter.SetIndicatorColor((value > 0.5f) ? UIColors.YELLOW : UIColors.CLEAR);
            }
        }

        public void PantographPowerUpdated(float value)
        {
            if (Provider.locoControls.mechanical.pantograph)
            {
                Provider.locoControls.mechanical.pantograph.SetIndicatorColor((value > 0.5f) ? UIColors.BLUE : UIColors.CLEAR);
            }
        }

        public void Pantograph2PowerUpdated(float value)
        {
            if (Provider.locoControls.mechanical.alerter)
            {
                Provider.locoControls.mechanical.alerter.SetIndicatorColor((value > 0.5f) ? UIColors.BLUE : UIColors.CLEAR);
            }
        }
    }
}
