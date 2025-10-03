namespace CCL.Creator
{
    internal static class MenuOrdering
    {
        public const int Simulation = 10;

        public static class MenuBar
        {
            public const int CarWizard = 100;
            public const int CargoWizard = 101;
            public const int LanguagePreview = 200;
            public const int Calculator = 300;
        }

        public static class Cab
        {
            public const int Control = 100;
            public const int Indicator = 101;
            public const int Lamp = 102;
            public const int Label = 103;
        }

        public static class Body
        {
            public const int Headlights = 200;
            public const int Cab = 201;
            public const int LicenseBlocker = 202;
        }

        public static class Interior
        {
            public const int Bed = 300;
        }

        public static class Particles
        {
            public const int Diesel = 0;
            public const int Steam = 100;
        }
    }
}
