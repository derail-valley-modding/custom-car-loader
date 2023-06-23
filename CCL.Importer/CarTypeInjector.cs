using CCL.Types;
using DV;
using System.Linq;

namespace CCL.Importer
{
    public static class CarTypeInjector
    {
        public static bool RegisterCarType(CustomCarType carType)
        {
            carType.kind = Globals.G.Types.CarKinds.First(k => k.id == carType.KindSelection.ToString());

            foreach (var livery in carType.liveries)
            {
                livery.parentType = carType;
            }

            Globals.G.Types.carTypes.Add(carType);

            TranslationInjector.AddTranslations(carType.localizationKey, carType.NameTranslations);
            
            foreach (var livery in carType.CustomLiveries)
            {
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
