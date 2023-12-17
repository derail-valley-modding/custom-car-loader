using CCL.Importer.Types;
using DV;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer
{
    public static class CarTypeInjector
    {
        public static readonly Dictionary<string, CCL_CarVariant> IdToLiveryMap =
            new Dictionary<string, CCL_CarVariant>();

        public static void SetupTypeLinks(CCL_CarType carType)
        {
            carType.kind = Globals.G.Types.CarKinds.First(k => string.Equals(k.id, carType.KindSelection.ToString(), System.StringComparison.OrdinalIgnoreCase));
            
            foreach (var livery in carType.liveries)
            {
                livery.parentType = carType;
            }
        }

        public static bool RegisterCarType(CCL_CarType carType)
        {
            Globals.G.Types.carTypes.Add(carType);

            CCLPlugin.Translations.AddTranslations(carType.localizationKey, carType.NameTranslations);
            
            foreach (var livery in carType.Variants)
            {
                IdToLiveryMap.Add(livery.id, livery);
                CCLPlugin.Translations.AddTranslations(livery.localizationKey, livery.NameTranslations);
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
