using CCL.Types;
using DV;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer
{
    public static class CarTypeInjector
    {
        public static readonly Dictionary<string, CustomLivery> IdToLiveryMap =
            new Dictionary<string, CustomLivery>();

        public static void SetupTypeLinks(CustomCarType carType)
        {
            carType.kind = Globals.G.Types.CarKinds.First(k => string.Equals(k.id, carType.KindSelection.ToString(), System.StringComparison.OrdinalIgnoreCase));

            foreach (var livery in carType.liveries)
            {
                livery.parentType = carType;
            }
        }

        public static bool RegisterCarType(CustomCarType carType)
        {
            Globals.G.Types.carTypes.Add(carType);

            TranslationInjector.AddTranslations(carType.localizationKey, carType.NameTranslations);
            
            foreach (var livery in carType.CustomLiveries)
            {
                IdToLiveryMap.Add(livery.id, livery);
                TranslationInjector.AddTranslations(livery.localizationKey, livery.NameTranslations);
            }

            CargoInjector.InjectLoadableCargos(carType);

            return true;
        }

        public static void ForceObjectModelUpdate()
        {
            Globals.G.Types.RecalculateCaches();

            //foreach (var spawner in Resources.FindObjectsOfTypeAll<CommsRadioCarSpawner>())
            //{
            //    spawner.UpdateCarLiveriesToSpawn(true);
            //}
        }
    }
}
