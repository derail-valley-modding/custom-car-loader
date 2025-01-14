using DV;
using DV.ThingTypes;

namespace CCL.Importer
{
    public static class QuickAccess
    {
        private static TrainCarLivery GetCarLivery(string id)
        {
            if (!Globals.G.Types.TryGetLivery(id, out var livery))
            {
                CCLPlugin.Error($"Failed to find car livery with ID {id}");
            }

            return livery;
        }

        private static TrainCarType_v2 GetCarType(string id)
        {
            if (!Globals.G.Types.TryGetCarType(id, out var car))
            {
                CCLPlugin.Error($"Failed to find car type with ID {id}");
            }

            return car;
        }

        public static class Locomotives
        {
            private static TrainCarLivery? s_de2;
            private static TrainCarLivery? s_de6;
            private static TrainCarLivery? s_dh4;
            private static TrainCarLivery? s_dm3;
            private static TrainCarLivery? s_s060;
            private static TrainCarLivery? s_s282a;
            private static TrainCarLivery? s_s282b;

            public static TrainCarLivery DE2 => Extensions.GetCached(ref s_de2, () => GetCarLivery("LocoDE2"));
            public static TrainCarLivery DE6 => Extensions.GetCached(ref s_de6, () => GetCarLivery("LocoDE6"));
            public static TrainCarLivery DH4 => Extensions.GetCached(ref s_dh4, () => GetCarLivery("LocoDH4"));
            public static TrainCarLivery DM3 => Extensions.GetCached(ref s_dm3, () => GetCarLivery("LocoDM3"));
            public static TrainCarLivery S060 => Extensions.GetCached(ref s_s060, () => GetCarLivery("LocoS060"));
            public static TrainCarLivery S282A => Extensions.GetCached(ref s_s282a, () => GetCarLivery("LocoS282A"));
            public static TrainCarLivery S282B => Extensions.GetCached(ref s_s282b, () => GetCarLivery("LocoS282B"));

            private static TrainCarLivery? s_de6slug;
            private static TrainCarLivery? s_microshunter;
            private static TrainCarLivery? s_dm1u;

            public static TrainCarLivery DE6Slug => Extensions.GetCached(ref s_de6slug, () => GetCarLivery("LocoDE6Slug"));
            public static TrainCarLivery Microshunter => Extensions.GetCached(ref s_microshunter, () => GetCarLivery("LocoMicroshunter"));
            public static TrainCarLivery DM1U => Extensions.GetCached(ref s_dm1u, () => GetCarLivery("LocoDM1U"));
        }

        public static class Wagons
        {
            private static TrainCarLivery? s_flatbed;
            private static TrainCarLivery? s_caboose;

            public static TrainCarLivery Flatbed => Extensions.GetCached(ref s_flatbed, () => GetCarLivery("FlatbedEmpty"));
            public static TrainCarLivery Caboose => Extensions.GetCached(ref s_caboose, () => GetCarLivery("CabooseRed"));
        }
    }
}
