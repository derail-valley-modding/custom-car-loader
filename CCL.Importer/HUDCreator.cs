using CCL.Types.Components.HUD;
using DV.Localization;
using DV.ThingTypes.TransitionHelpers;
using DV.UI.LocoHUD;
using UnityEngine;

namespace CCL.Importer
{
    internal class HUDCreator
    {
        // WARNING
        // LOTS OF HARDCODED NAMES AND VALUES
        // PROCEED AT YOUR OWN DISCRETION

        private static string _path = "bg/Controls/HUDSlot";

        public static GameObject CreateHUD(VanillaHudLayout layout)
        {
            // S060 has the most complete HUD.
            GameObject hud = Object.Instantiate(DV.ThingTypes.TrainCarType.LocoS060.ToV2().parentType.hudPrefab);
            HUDLocoControls controls = hud.GetComponent<HUDLocoControls>();

            BasicControls(layout.Settings, controls);
            BrakeControls(layout.Settings, controls);
            SteamControls(layout.Settings, controls);
            CabControls(layout.Settings, controls);
            MechanicalControls(layout.Settings, controls);

            return hud;
        }

        private static string GetPath(int index)
        {
            if (index == 0)
            {
                return _path;
            }
            
            return $"{_path} ({index})";
        }

        private static void BasicControls(VanillaHudSettings settings, HUDLocoControls hud)
        {
            hud.basicControls.ampMeter.gameObject.SetActive(settings.BasicControls[0].Value1 == 1);
            hud.basicControls.throttle.gameObject.SetActive(settings.BasicControls[0].Value2 != 0);

            // Rename regulator to throttle.
            if (settings.BasicControls[0].Value2 != 2)
            {
                hud.basicControls.throttle.name = "Throttle";
                hud.basicControls.throttle.GetComponentInChildren<Localize>().key = "car/throttle";
            }

            hud.basicControls.tmTempMeter.gameObject.SetActive(settings.BasicControls[1].Value1 == 1);
            hud.basicControls.oilTempMeter.gameObject.SetActive(settings.BasicControls[1].Value1 == 2);

            // Fix oil temp position.
            if (settings.BasicControls[0].Value1 == 2)
            {
                hud.basicControls.oilTempMeter.transform.position = hud.basicControls.tmTempMeter.transform.position;
            }

            hud.basicControls.reverser.gameObject.SetActive(settings.BasicControls[1].Value2 != 0);

            // Rename cutoff to reverser.
            if (settings.BasicControls[0].Value2 != 2)
            {
                hud.basicControls.reverser.name = "Reverser";
                hud.basicControls.reverser.GetComponentInChildren<Localize>().key = "car/reverser";
            }

            hud.basicControls.gearboxA.gameObject.SetActive(settings.BasicControls[2].Value1 == 1);

            hud.basicControls.speedMeter.gameObject.SetActive(settings.BasicControls[3].Value1 == 1);
            hud.basicControls.gearboxB.gameObject.SetActive(settings.BasicControls[3].Value2 == 1);

            hud.basicControls.rpmMeter.gameObject.SetActive(settings.BasicControls[4].Value1 == 1);
            hud.basicControls.turbineRpmMeter.gameObject.SetActive(settings.BasicControls[4].Value2 == 1);
            hud.basicControls.voltageMeter.gameObject.SetActive(settings.BasicControls[4].Value2 == 2);
            hud.basicControls.powerMeter.gameObject.SetActive(settings.BasicControls[4].Value3 == 1);

            hud.basicControls.wheelSlipIndicator.gameObject.SetActive(settings.BasicControls[5].Value1 == 1);
            hud.basicControls.sand.gameObject.SetActive(settings.BasicControls[5].Value2 == 1);
        }

        private static void BrakeControls(VanillaHudSettings settings, HUDLocoControls hud)
        {
            hud.braking.brakePipeMeter.gameObject.SetActive(settings.BrakeControls[0].Value1 == 1);

            if (settings.BrakeControls[0].Value2 == 1)
            {
                hud.braking.trainBrake.gameObject.SetActive(false);
                hud.braking.trainBrake = hud.braking.trainBrake.transform.parent.Find("TrainBrakeSelfLapping").GetComponent<LocoHUDControlBase>();
                hud.braking.trainBrake.gameObject.SetActive(true);
            }
            else
            {
                hud.braking.trainBrake.gameObject.SetActive(settings.BrakeControls[0].Value2 == 2);
            }

            hud.braking.mainResMeter.gameObject.SetActive(settings.BrakeControls[1].Value1 == 1);
            hud.braking.indBrake.gameObject.SetActive(settings.BrakeControls[1].Value2 == 1);

            hud.braking.brakeCylMeter.gameObject.SetActive(settings.BrakeControls[2].Value1 == 1);
            hud.braking.dynBrake.gameObject.SetActive(settings.BrakeControls[2].Value2 == 1);

            hud.braking.releaseCyl.gameObject.SetActive(settings.BrakeControls[3].Value1 == 1);
            hud.braking.handbrake.gameObject.SetActive(settings.BrakeControls[3].Value2 == 1);
        }

        private static void SteamControls(VanillaHudSettings settings, HUDLocoControls hud)
        {
            hud.steam.steamMeter.gameObject.SetActive(settings.SteamControls[0].Value1 == 1);
            hud.steam.cylCock.gameObject.SetActive(settings.SteamControls[0].Value2 == 1);

            hud.steam.locoWaterMeter.gameObject.SetActive(settings.SteamControls[1].Value1 == 1);
            hud.steam.injector.gameObject.SetActive(settings.SteamControls[1].Value2 == 1);

            hud.steam.locoCoalMeter.gameObject.SetActive(settings.SteamControls[2].Value1 == 1);
            hud.steam.damper.gameObject.SetActive(settings.SteamControls[2].Value2 == 1);

            hud.steam.fireTemp.gameObject.SetActive(settings.SteamControls[3].Value1 == 1);
            hud.steam.blower.gameObject.SetActive(settings.SteamControls[3].Value2 == 1);

            hud.steam.shovel.gameObject.SetActive(settings.SteamControls[4].Value1 == 1);
            hud.steam.firedoor.gameObject.SetActive(settings.SteamControls[4].Value2 == 1);

            hud.steam.lightFirebox.gameObject.SetActive(settings.SteamControls[5].Value1 == 1);
            hud.steam.blowdown.gameObject.SetActive(settings.SteamControls[5].Value2 == 1);

            hud.steam.coalDump.gameObject.SetActive(settings.SteamControls[6].Value1 == 1);

            hud.steam.dynamo.gameObject.SetActive(settings.SteamControls[7].Value1 == 1);
            hud.steam.airPump.gameObject.SetActive(settings.SteamControls[7].Value2 == 1);
            hud.steam.lubricator.gameObject.SetActive(settings.SteamControls[7].Value3 == 1);
        }

        private static void CabControls(VanillaHudSettings settings, HUDLocoControls hud)
        {
            hud.cab.fuelLevelMeter.gameObject.SetActive(settings.CabControls[0].Value1 == 1);
            hud.cab.wipers.gameObject.SetActive(settings.CabControls[0].Value2 == 1);
            hud.cab.indWipers1.gameObject.SetActive(settings.CabControls[0].Value2 == 2);
            hud.cab.indWipers2.gameObject.SetActive(settings.CabControls[0].Value2 == 2);

            hud.cab.oilLevelMeter.gameObject.SetActive(settings.CabControls[1].Value1 == 1);

            hud.cab.sandMeter.gameObject.SetActive(settings.CabControls[2].Value1 == 1);
        }

        private static void MechanicalControls(VanillaHudSettings settings, HUDLocoControls hud)
        {
            hud.mechanical.pantograph.gameObject.SetActive(settings.MechanicalControls[0].Value1 == 1);
            hud.mechanical.cabOrient.gameObject.SetActive(settings.MechanicalControls[0].Value2 == 1);

            hud.mechanical.tmOfflineIndicator.gameObject.SetActive(settings.MechanicalControls[1].Value1 == 1);
            hud.mechanical.starterFuse.gameObject.SetActive(settings.MechanicalControls[1].Value2 == 1);
            hud.mechanical.electricsFuse.gameObject.SetActive(settings.MechanicalControls[1].Value3 == 1);
            hud.mechanical.tractionMotorFuse.gameObject.SetActive(settings.MechanicalControls[1].Value4 == 1);

            hud.mechanical.alerter.gameObject.SetActive(settings.MechanicalControls[2].Value1 == 1);
            hud.mechanical.starterControl.gameObject.SetActive(settings.MechanicalControls[2].Value2 == 1);
            hud.mechanical.fuelCutoff.gameObject.SetActive(settings.MechanicalControls[2].Value3 == 1);
        }
    }
}
