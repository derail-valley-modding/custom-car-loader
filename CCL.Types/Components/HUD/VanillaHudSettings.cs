using System;

namespace CCL.Types.Components.HUD
{
    [Serializable]
    public class VanillaHudSettings
    {
        public const int BasicControlCount = 6;
        public const int BrakeControlCount = 4;
        public const int SteamControlCount = 8;
        public const int CabControlCount = 5;
        public const int MechanicalControlCount = 3;

        [Serializable]
        public class Slot
        {
            public int Value1 = 0;
            public int Value2 = 0;
            public int Value3 = 0;
            public int Value4 = 0;
        }

        public Slot[] BasicControls;
        public Slot[] BrakeControls;
        public Slot[] SteamControls;
        public Slot[] CabControls;
        public Slot[] MechanicalControls;

        public VanillaHudSettings()
        {
            BasicControls = InitializeArray(BasicControlCount);
            BrakeControls = InitializeArray(BrakeControlCount);
            SteamControls = InitializeArray(SteamControlCount);
            CabControls = InitializeArray(CabControlCount);
            MechanicalControls = InitializeArray(MechanicalControlCount);
        }

        private static Slot[] InitializeArray(int count)
        {
            var MechanicalControls = new Slot[count];

            for (int i = 0; i < count; i++)
            {
                MechanicalControls[i] = new Slot();
            }

            return MechanicalControls;
        }

        public static void SetToDE(VanillaHudSettings settings)
        {
            // Amps, Throttle
            settings.BasicControls[0].Value1 = 1;
            settings.BasicControls[0].Value2 = 1;
            // TMTemp, Reverser
            settings.BasicControls[1].Value1 = 1;
            settings.BasicControls[1].Value2 = 1;
            // None
            settings.BasicControls[2].Value1 = 0;
            // Speedometer, None
            settings.BasicControls[3].Value1 = 1;
            settings.BasicControls[3].Value2 = 0;
            // RPM, None, None
            settings.BasicControls[4].Value1 = 1;
            settings.BasicControls[4].Value2 = 0;
            settings.BasicControls[4].Value3 = 0;
            // Wheelslip, Sand
            settings.BasicControls[5].Value1 = 1;
            settings.BasicControls[5].Value2 = 1;

            // Brake Pipe
            settings.BrakeControls[0].Value1 = 1;
            // Main Res, Independent
            settings.BrakeControls[1].Value1 = 1;
            settings.BrakeControls[1].Value2 = 1;
            // Brake Cylinder, Dynamic
            settings.BrakeControls[2].Value1 = 1;
            settings.BrakeControls[2].Value2 = 1;
            // Release Cylinder, Handbrake, None
            settings.BrakeControls[3].Value1 = 1;
            settings.BrakeControls[3].Value2 = 1;
            settings.BrakeControls[3].Value3 = 0;

            // None, None
            settings.SteamControls[0].Value1 = 0;
            settings.SteamControls[0].Value2 = 0;
            // None, None
            settings.SteamControls[1].Value1 = 0;
            settings.SteamControls[1].Value2 = 0;
            // None, None
            settings.SteamControls[2].Value1 = 0;
            settings.SteamControls[2].Value2 = 0;
            // None, None
            settings.SteamControls[3].Value1 = 0;
            settings.SteamControls[3].Value2 = 0;
            // None, None
            settings.SteamControls[4].Value1 = 0;
            settings.SteamControls[4].Value2 = 0;
            // None, None
            settings.SteamControls[5].Value1 = 0;
            settings.SteamControls[5].Value2 = 0;
            // None
            settings.SteamControls[6].Value1 = 0;
            // None, None, None
            settings.SteamControls[7].Value1 = 0;
            settings.SteamControls[7].Value2 = 0;
            settings.SteamControls[7].Value3 = 0;

            // Fuel Level
            settings.CabControls[0].Value1 = 1;
            // Oil Level
            settings.CabControls[1].Value1 = 1;
            // Sand Level
            settings.CabControls[2].Value1 = 1;
            // None OR Bell
            settings.CabControls[3].Value1 = settings.CabControls[3].Value1 == 2 ? 1 : settings.CabControls[3].Value1;
            // Horn
            settings.CabControls[4].Value1 = 1;

            // None, None
            settings.MechanicalControls[0].Value1 = 0;
            settings.MechanicalControls[0].Value2 = 0;
            // TM Offline, Starter Fuse, Electrics Fuse, Traction Motor Fuse
            settings.MechanicalControls[1].Value1 = 1;
            settings.MechanicalControls[1].Value2 = 1;
            settings.MechanicalControls[1].Value3 = 1;
            settings.MechanicalControls[1].Value4 = 1;
            // None, Starter, Fuel Cutoff
            settings.MechanicalControls[2].Value1 = 0;
            settings.MechanicalControls[2].Value2 = 1;
            settings.MechanicalControls[2].Value3 = 1;
        }

        public static void SetToDH(VanillaHudSettings settings)
        {
            // None, Throttle
            settings.BasicControls[0].Value1 = 0;
            settings.BasicControls[0].Value2 = 1;
            // OilTemp, Reverser
            settings.BasicControls[1].Value1 = 2;
            settings.BasicControls[1].Value2 = 1;
            // None
            settings.BasicControls[2].Value1 = 0;
            // Speedometer, None
            settings.BasicControls[3].Value1 = 1;
            settings.BasicControls[3].Value2 = 0;
            // RPM, Turbine RPM, None
            settings.BasicControls[4].Value1 = 1;
            settings.BasicControls[4].Value2 = 1;
            settings.BasicControls[4].Value3 = 0;
            // Wheelslip, Sand
            settings.BasicControls[5].Value1 = 1;
            settings.BasicControls[5].Value2 = 1;

            // Brake Pipe
            settings.BrakeControls[0].Value1 = 1;
            // Main Res, Independent
            settings.BrakeControls[1].Value1 = 1;
            settings.BrakeControls[1].Value2 = 1;
            // Brake Cylinder, Dynamic
            settings.BrakeControls[2].Value1 = 1;
            settings.BrakeControls[2].Value2 = 1;
            // Release Cylinder, Handbrake, None
            settings.BrakeControls[3].Value1 = 1;
            settings.BrakeControls[3].Value2 = 1;
            settings.BrakeControls[3].Value3 = 0;

            // None, None
            settings.SteamControls[0].Value1 = 0;
            settings.SteamControls[0].Value2 = 0;
            // None, None
            settings.SteamControls[1].Value1 = 0;
            settings.SteamControls[1].Value2 = 0;
            // None, None
            settings.SteamControls[2].Value1 = 0;
            settings.SteamControls[2].Value2 = 0;
            // None, None
            settings.SteamControls[3].Value1 = 0;
            settings.SteamControls[3].Value2 = 0;
            // None, None
            settings.SteamControls[4].Value1 = 0;
            settings.SteamControls[4].Value2 = 0;
            // None, None
            settings.SteamControls[5].Value1 = 0;
            settings.SteamControls[5].Value2 = 0;
            // None
            settings.SteamControls[6].Value1 = 0;
            // None, None, None
            settings.SteamControls[7].Value1 = 0;
            settings.SteamControls[7].Value2 = 0;
            settings.SteamControls[7].Value3 = 0;

            // Fuel Level
            settings.CabControls[0].Value1 = 1;
            // Oil Level
            settings.CabControls[1].Value1 = 1;
            // Sand Level
            settings.CabControls[2].Value1 = 1;
            // None OR Bell
            settings.CabControls[3].Value1 = settings.CabControls[3].Value1 == 2 ? 1 : settings.CabControls[3].Value1;
            // Horn
            settings.CabControls[4].Value1 = 1;

            // None, None
            settings.MechanicalControls[0].Value1 = 0;
            settings.MechanicalControls[0].Value2 = 0;
            // None, Starter Fuse, Electrics Fuse, None
            settings.MechanicalControls[1].Value1 = 0;
            settings.MechanicalControls[1].Value2 = 1;
            settings.MechanicalControls[1].Value3 = 1;
            settings.MechanicalControls[1].Value4 = 0;
            // None, Starter, Fuel Cutoff
            settings.MechanicalControls[2].Value1 = 0;
            settings.MechanicalControls[2].Value2 = 1;
            settings.MechanicalControls[2].Value3 = 1;
        }

        public static void SetToDM(VanillaHudSettings settings)
        {
            // None, Throttle
            settings.BasicControls[0].Value1 = 0;
            settings.BasicControls[0].Value2 = 1;
            // OilTemp, Reverser
            settings.BasicControls[1].Value1 = 2;
            settings.BasicControls[1].Value2 = 1;
            // GearboxA
            settings.BasicControls[2].Value1 = 1;
            // Speedometer, GearboxB
            settings.BasicControls[3].Value1 = 1;
            settings.BasicControls[3].Value2 = 1;
            // RPM, None, None
            settings.BasicControls[4].Value1 = 1;
            settings.BasicControls[4].Value2 = 0;
            settings.BasicControls[4].Value3 = 0;
            // Wheelslip, Sand
            settings.BasicControls[5].Value1 = 1;
            settings.BasicControls[5].Value2 = 1;

            // Brake Pipe
            settings.BrakeControls[0].Value1 = 1;
            // Main Res, Independent
            settings.BrakeControls[1].Value1 = 1;
            settings.BrakeControls[1].Value2 = 1;
            // Brake Cylinder, Dynamic
            settings.BrakeControls[2].Value1 = 1;
            settings.BrakeControls[2].Value2 = 1;
            // Release Cylinder, Handbrake, None
            settings.BrakeControls[3].Value1 = 1;
            settings.BrakeControls[3].Value2 = 1;
            settings.BrakeControls[3].Value3 = 0;

            // None, None
            settings.SteamControls[0].Value1 = 0;
            settings.SteamControls[0].Value2 = 0;
            // None, None
            settings.SteamControls[1].Value1 = 0;
            settings.SteamControls[1].Value2 = 0;
            // None, None
            settings.SteamControls[2].Value1 = 0;
            settings.SteamControls[2].Value2 = 0;
            // None, None
            settings.SteamControls[3].Value1 = 0;
            settings.SteamControls[3].Value2 = 0;
            // None, None
            settings.SteamControls[4].Value1 = 0;
            settings.SteamControls[4].Value2 = 0;
            // None, None
            settings.SteamControls[5].Value1 = 0;
            settings.SteamControls[5].Value2 = 0;
            // None
            settings.SteamControls[6].Value1 = 0;
            // None, None, None
            settings.SteamControls[7].Value1 = 0;
            settings.SteamControls[7].Value2 = 0;
            settings.SteamControls[7].Value3 = 0;

            // Fuel Level
            settings.CabControls[0].Value1 = 1;
            // Oil Level
            settings.CabControls[1].Value1 = 1;
            // Sand Level
            settings.CabControls[2].Value1 = 1;
            // None OR Bell
            settings.CabControls[3].Value1 = settings.CabControls[3].Value1 == 2 ? 1 : settings.CabControls[3].Value1;
            // Horn
            settings.CabControls[4].Value1 = 1;

            // None, None
            settings.MechanicalControls[0].Value1 = 0;
            settings.MechanicalControls[0].Value2 = 0;
            // None, Starter Fuse, Electrics Fuse, None
            settings.MechanicalControls[1].Value1 = 0;
            settings.MechanicalControls[1].Value2 = 1;
            settings.MechanicalControls[1].Value3 = 1;
            settings.MechanicalControls[1].Value4 = 0;
            // None, Starter, Fuel Cutoff
            settings.MechanicalControls[2].Value1 = 0;
            settings.MechanicalControls[2].Value2 = 1;
            settings.MechanicalControls[2].Value3 = 1;
        }

        public static void SetToS(VanillaHudSettings settings)
        {
            // None, Regulator
            settings.BasicControls[0].Value1 = 0;
            settings.BasicControls[0].Value2 = 1;
            // None, Cutoff
            settings.BasicControls[1].Value1 = 0;
            settings.BasicControls[1].Value2 = 2;
            // None
            settings.BasicControls[2].Value1 = 0;
            // Speedometer, None
            settings.BasicControls[3].Value1 = 1;
            settings.BasicControls[3].Value2 = 0;
            // None, None, None
            settings.BasicControls[4].Value1 = 0;
            settings.BasicControls[4].Value2 = 0;
            settings.BasicControls[4].Value3 = 0;
            // None, Sand
            settings.BasicControls[5].Value1 = 0;
            settings.BasicControls[5].Value2 = 1;

            // Brake Pipe
            settings.BrakeControls[0].Value1 = 1;
            // Main Res, Independent
            settings.BrakeControls[1].Value1 = 1;
            settings.BrakeControls[1].Value2 = 1;
            // Brake Cylinder, None
            settings.BrakeControls[2].Value1 = 1;
            settings.BrakeControls[2].Value2 = 0;
            // Release Cylinder, Handbrake, None
            settings.BrakeControls[3].Value1 = 1;
            settings.BrakeControls[3].Value2 = 1;
            settings.BrakeControls[3].Value3 = 0;

            // Steam Meter, Cylinder Cocks
            settings.SteamControls[0].Value1 = 1;
            settings.SteamControls[0].Value2 = 1;
            // Loco Water Meter, Injector
            settings.SteamControls[1].Value1 = 1;
            settings.SteamControls[1].Value2 = 1;
            // Loco Coal Meter, Damper
            settings.SteamControls[2].Value1 = 1;
            settings.SteamControls[2].Value2 = 1;
            // Fire Temperature, Blower
            settings.SteamControls[3].Value1 = 1;
            settings.SteamControls[3].Value2 = 1;
            // Shovel, Firedoor
            settings.SteamControls[4].Value1 = 1;
            settings.SteamControls[4].Value2 = 1;
            // Light Firebox, Water Dump
            settings.SteamControls[5].Value1 = 1;
            settings.SteamControls[5].Value2 = 1;
            // Coal Dump
            settings.SteamControls[6].Value1 = 1;
            // Dynamo, Air Pump, Lubricator
            settings.SteamControls[7].Value1 = 1;
            settings.SteamControls[7].Value2 = 1;
            settings.SteamControls[7].Value3 = 1;

            // None
            settings.CabControls[0].Value1 = 0;
            // Oil Level
            settings.CabControls[1].Value1 = 1;
            // Sand Level
            settings.CabControls[2].Value1 = 1;
            // Tender Water
            settings.CabControls[3].Value1 = 2;
            // Whistle + Tender Coal
            settings.CabControls[4].Value1 = 2;

            // None, None
            settings.MechanicalControls[0].Value1 = 0;
            settings.MechanicalControls[0].Value2 = 0;
            // None, None, None, None
            settings.MechanicalControls[1].Value1 = 0;
            settings.MechanicalControls[1].Value2 = 0;
            settings.MechanicalControls[1].Value3 = 0;
            settings.MechanicalControls[1].Value4 = 0;
            // None, None, None
            settings.MechanicalControls[2].Value1 = 0;
            settings.MechanicalControls[2].Value2 = 0;
            settings.MechanicalControls[2].Value3 = 0;
        }
    }
}
