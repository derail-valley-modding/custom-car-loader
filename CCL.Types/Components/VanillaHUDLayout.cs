using CCL.Types.Json;
using System;
using UnityEngine;
using static CCL.Types.Components.BasicControls;
using static CCL.Types.Components.Cab;
using static CCL.Types.Components.VanillaHUDLayout;

namespace CCL.Types.Components
{
    // =========================
    //
    //      HERE BE DRAGONS
    //
    //         HARDCODED
    //       HUD  ELEMENTS
    //
    // =========================
    public class VanillaHUDLayout : MonoBehaviour, ICustomSerialized
    {
        public enum ShouldDisplay
        {
            None,
            Display
        }

        [Tooltip("The powertrain of this vehicle")]
        public string Powertrain = "";
        [Space]
        public BasicControls BasicControls = new BasicControls();
        public Braking Braking = new Braking();
        public Steam Steam = new Steam();
        public Cab Cab = new Cab();
        public Mechanical Mechanical = new Mechanical();

        [SerializeField, HideInInspector]
        private string? _basic;
        [SerializeField, HideInInspector]
        private string? _brake;
        [SerializeField, HideInInspector]
        private string? _steam;
        [SerializeField, HideInInspector]
        private string? _cab;
        [SerializeField, HideInInspector]
        private string? _mecha;

        public void AfterImport()
        {
            BasicControls = JSONObject.FromJson(_basic, () => new BasicControls());
            Braking = JSONObject.FromJson(_brake, () => new Braking());
            Steam = JSONObject.FromJson(_steam, () => new Steam());
            Cab = JSONObject.FromJson(_cab, () => new Cab());
            Mechanical = JSONObject.FromJson(_mecha, () => new Mechanical());
        }

        public void OnValidate()
        {
            _basic = JSONObject.ToJson(BasicControls);
            _brake = JSONObject.ToJson(Braking);
            _steam = JSONObject.ToJson(Steam);
            _cab = JSONObject.ToJson(Cab);
            _mecha = JSONObject.ToJson(Mechanical);
        }

        public void SetToDE()
        {
            Powertrain = "DE";

            // Basic.
            BasicControls.AmpMeter = ShouldDisplay.Display;
            BasicControls.Throttle = Slot0B.Throttle;

            BasicControls.TMOrOilTemp = Slot1A.TMTemp;
            BasicControls.Reverser = Slot1B.Reverser;

            BasicControls.GearboxA = ShouldDisplay.None;

            BasicControls.GearboxB = ShouldDisplay.None;

            BasicControls.RPM = ShouldDisplay.Display;
            BasicControls.TurbineOrVoltage = Slot4B.None;
            BasicControls.Power = ShouldDisplay.None;

            // All Steam to none.
            Steam = new Steam();

            // Cab.
            Cab.FuelDisplay = Slot21A.FuelLevel;

            Cab.BellOrWater = Cab.BellOrWater == Slot24A.TenderWater ? Slot24A.None : Cab.BellOrWater;

            // Mechanical.
            Mechanical.Pantograph = ShouldDisplay.None;

            Mechanical.TMOffline = ShouldDisplay.Display;
            Mechanical.StarterFuse = ShouldDisplay.Display;
            Mechanical.ElectricsFuse = ShouldDisplay.Display;
            Mechanical.TractionMotorFuse = ShouldDisplay.Display;

            Mechanical.Starter = ShouldDisplay.Display;
            Mechanical.FuelCutoff = ShouldDisplay.Display;
        }

        public void SetToDH()
        {
            Powertrain = "DH";

            // Basic.
            BasicControls.AmpMeter = ShouldDisplay.None;
            BasicControls.Throttle = Slot0B.Throttle;

            BasicControls.TMOrOilTemp = Slot1A.OilTemp;
            BasicControls.Reverser = Slot1B.Reverser;

            BasicControls.GearboxA = ShouldDisplay.None;

            BasicControls.GearboxB = ShouldDisplay.None;

            BasicControls.RPM = ShouldDisplay.Display;
            BasicControls.TurbineOrVoltage = Slot4B.TurbineRPM;
            BasicControls.Power = ShouldDisplay.None;

            // All Steam to none.
            Steam = new Steam();

            // Cab.
            Cab.FuelDisplay = Slot21A.FuelLevel;

            Cab.OilLevel = ShouldDisplay.Display;

            Cab.BellOrWater = Cab.BellOrWater == Slot24A.TenderWater ? Slot24A.None : Cab.BellOrWater;

            // Mechanical.
            Mechanical.Pantograph = ShouldDisplay.None;

            Mechanical.TMOffline = ShouldDisplay.None;
            Mechanical.StarterFuse = ShouldDisplay.Display;
            Mechanical.ElectricsFuse = ShouldDisplay.Display;
            Mechanical.TractionMotorFuse = ShouldDisplay.None;

            Mechanical.Starter = ShouldDisplay.Display;
            Mechanical.FuelCutoff = ShouldDisplay.Display;
        }

        public void SetToDM()
        {
            Powertrain = "DM";

            // Basic.
            BasicControls.AmpMeter = ShouldDisplay.None;
            BasicControls.Throttle = Slot0B.Throttle;

            BasicControls.TMOrOilTemp = Slot1A.OilTemp;
            BasicControls.Reverser = Slot1B.Reverser;

            BasicControls.GearboxA = ShouldDisplay.Display;

            BasicControls.GearboxB = ShouldDisplay.Display;

            BasicControls.RPM = ShouldDisplay.Display;
            BasicControls.TurbineOrVoltage = Slot4B.None;
            BasicControls.Power = ShouldDisplay.None;

            // All Steam to none.
            Steam = new Steam();

            // Cab.
            Cab.FuelDisplay = Slot21A.FuelLevel;

            Cab.OilLevel = ShouldDisplay.Display;

            Cab.BellOrWater = Cab.BellOrWater == Slot24A.TenderWater ? Slot24A.None : Cab.BellOrWater;

            // Mechanical.
            Mechanical.Pantograph = ShouldDisplay.None;

            Mechanical.TMOffline = ShouldDisplay.None;
            Mechanical.StarterFuse = ShouldDisplay.Display;
            Mechanical.ElectricsFuse = ShouldDisplay.Display;
            Mechanical.TractionMotorFuse = ShouldDisplay.None;

            Mechanical.Starter = ShouldDisplay.Display;
            Mechanical.FuelCutoff = ShouldDisplay.Display;
        }

        public void SetToBE()
        {
            Powertrain = "BE";

            // Basic.
            BasicControls.AmpMeter = ShouldDisplay.Display;
            BasicControls.Throttle = Slot0B.Throttle;

            BasicControls.TMOrOilTemp = Slot1A.TMTemp;
            BasicControls.Reverser = Slot1B.Reverser;

            BasicControls.GearboxA = ShouldDisplay.None;

            BasicControls.GearboxB = ShouldDisplay.None;

            BasicControls.RPM = ShouldDisplay.None;
            BasicControls.TurbineOrVoltage = Slot4B.Voltage;
            BasicControls.Power = ShouldDisplay.None;

            // All Steam to none.
            Steam = new Steam();

            // Cab.
            Cab.FuelDisplay = Slot21A.BatteryLevel;

            Cab.BellOrWater = Cab.BellOrWater == Slot24A.TenderWater ? Slot24A.None : Cab.BellOrWater;

            // Mechanical.
            Mechanical.Pantograph = ShouldDisplay.None;

            Mechanical.TMOffline = ShouldDisplay.Display;
            Mechanical.StarterFuse = ShouldDisplay.None;
            Mechanical.ElectricsFuse = ShouldDisplay.Display;
            Mechanical.TractionMotorFuse = ShouldDisplay.Display;

            Mechanical.Starter = ShouldDisplay.None;
            Mechanical.FuelCutoff = ShouldDisplay.None;
        }

        public void SetToS()
        {
            Powertrain = "S";

            // Basic.
            BasicControls.AmpMeter = ShouldDisplay.None;
            BasicControls.Throttle = Slot0B.Regulator;

            BasicControls.TMOrOilTemp = Slot1A.None;
            BasicControls.Reverser = Slot1B.Cutoff;

            BasicControls.GearboxA = ShouldDisplay.None;

            BasicControls.GearboxB = ShouldDisplay.None;

            BasicControls.RPM = ShouldDisplay.None;
            BasicControls.TurbineOrVoltage = Slot4B.None;
            BasicControls.Power = ShouldDisplay.None;

            // Steam.
            Steam.SteamMeter = ShouldDisplay.Display;
            Steam.CylinderCocks = ShouldDisplay.Display;

            Steam.BoilerWater = ShouldDisplay.Display;
            Steam.Injector = ShouldDisplay.Display;

            Steam.FireboxCoal = ShouldDisplay.Display;
            Steam.Damper = ShouldDisplay.Display;

            Steam.FireTemperature = ShouldDisplay.Display;
            Steam.Blower = ShouldDisplay.Display;

            Steam.Shovel = ShouldDisplay.Display;
            Steam.Firedoor = ShouldDisplay.Display;

            Steam.LightFirebox = ShouldDisplay.Display;
            Steam.Blowdown = ShouldDisplay.Display;

            Steam.FuelDump = ShouldDisplay.Display;

            Steam.Dynamo = ShouldDisplay.Display;
            Steam.AirPump = ShouldDisplay.Display;
            Steam.Lubricator = ShouldDisplay.Display;

            // Cab.
            Cab.FuelDisplay = Slot21A.None;

            Cab.OilLevel = ShouldDisplay.Display;

            Cab.BellOrWater = Slot24A.TenderWater;

            // Mechanical.
            Mechanical.Pantograph = ShouldDisplay.None;

            Mechanical.TMOffline = ShouldDisplay.None;
            Mechanical.StarterFuse = ShouldDisplay.None;
            Mechanical.TractionMotorFuse = ShouldDisplay.None;

            Mechanical.Starter = ShouldDisplay.None;
            Mechanical.FuelCutoff = ShouldDisplay.None;
        }
    }

    [Serializable]
    public class BasicControls
    {
        public enum Slot0B { None, Throttle, Regulator }
        public enum Slot1A { None, TMTemp, OilTemp }
        public enum Slot1B { None, Reverser, Cutoff }
        public enum Slot4B { None, TurbineRPM, Voltage }

        [Header("Slot 1")]
        public ShouldDisplay AmpMeter;
        public Slot0B Throttle;

        [Header("Slot 2")]
        public Slot1A TMOrOilTemp;
        public Slot1B Reverser;

        [Header("Slot 3")]
        public ShouldDisplay GearboxA;

        [Header("Slot 4")]
        public ShouldDisplay Speedometer;
        public ShouldDisplay GearboxB;

        [Header("Slot 5")]
        public ShouldDisplay RPM;
        public Slot4B TurbineOrVoltage;
        public ShouldDisplay Power;

        [Header("Slot 6")]
        public ShouldDisplay WheelslipIndicator;
        public ShouldDisplay Sander;
    }

    [Serializable]
    public class Braking
    {
        public enum Slot7B { None, SelfLapping, NonSelfLapping }

        [Header("Slot 7")]
        public ShouldDisplay BrakePipe;
        public Slot7B BrakeType;

        [Header("Slot 8")]
        public ShouldDisplay MainReservoir;
        public ShouldDisplay IndependentBrake;

        [Header("Slot 9")]
        public ShouldDisplay BrakeCylinder;
        public ShouldDisplay DynamicBrake;

        [Header("Slot 10")]
        public ShouldDisplay ReleaseCylinder;
        public ShouldDisplay Handbrake;
    }

    [Serializable]
    public class Steam
    {
        [Header("Slot 11")]
        public ShouldDisplay SteamMeter;
        public ShouldDisplay CylinderCocks;

        [Header("Slot 12")]
        public ShouldDisplay BoilerWater;
        public ShouldDisplay Injector;

        [Header("Slot 13")]
        public ShouldDisplay FireboxCoal;
        public ShouldDisplay Damper;

        [Header("Slot 14")]
        public ShouldDisplay FireTemperature;
        public ShouldDisplay Blower;

        [Header("Slot 15")]
        public ShouldDisplay Shovel;
        public ShouldDisplay Firedoor;

        [Header("Slot 16")]
        public ShouldDisplay LightFirebox;
        public ShouldDisplay Blowdown;

        [Header("Slot 17")]
        public ShouldDisplay FuelDump;

        [Header("Slot 18")]
        public ShouldDisplay Dynamo;
        public ShouldDisplay AirPump;
        public ShouldDisplay Lubricator;
    }

    [Serializable]
    public class Cab
    {
        public enum Slot21A { None, FuelLevel, BatteryLevel }
        public enum Slot21B { None, SimpleWipers, DM3Wipers }
        public enum Slot22B { None, CabLightsSlider, DashAndCabLight }
        public enum Slot23B { None, HeadlightsSlider, DM3Headlights }
        public enum Slot24A { None, BellButton, TenderWater }
        public enum Slot24B { None, HeadlightsSlider, DM3Headlights, BellSlider }
        public enum Slot25B { None, Horn, Whistle }

        [Header("Slot 19")]
        public Slot21A FuelDisplay;
        public Slot21B Wipers;

        [Header("Slot 20")]
        public ShouldDisplay OilLevel;
        public Slot22B CabLightStyle;

        [Header("Slot 21")]
        public ShouldDisplay SandLevel;
        public Slot23B Headlights1;

        [Header("Slot 22")]
        public Slot24A BellOrWater;
        public Slot24B Headlights2;

        [Header("Slot 23")]
        public ShouldDisplay TenderCoal;
        public Slot25B HornStyle;
    }

    [Serializable]
    public class Mechanical
    {
        [Header("Slot 24")]
        public ShouldDisplay Pantograph;
        public ShouldDisplay CabOrientation;

        [Header("Slot 25")]
        public ShouldDisplay TMOffline;
        public ShouldDisplay StarterFuse;
        public ShouldDisplay ElectricsFuse;
        public ShouldDisplay TractionMotorFuse;

        [Header("Slot 26")]
        public ShouldDisplay Alerter;
        public ShouldDisplay Starter;
        public ShouldDisplay FuelCutoff;
    }
}
