using CCL.Types;
using CCL.Types.Proxies.Audio;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using UnityEngine;
using UnityEngine.Audio;

namespace CCL.Importer
{
    //[HarmonyPatch(typeof(Enum), nameof(Enum.IsDefined))]
    //public static class EnumPatch
    //{
    //    public static bool Prefix(Type enumType, object value, ref bool __result)
    //    {
    //        if ((enumType == typeof(TrainCarType)) && (value is TrainCarType carType))
    //        {
    //            __result = CarTypeInjector.IsCustomTypeRegistered(carType);
    //            if (__result) return false;
    //        }
    //        else if ((enumType == typeof(CargoType)) && (value is CargoType cargoType))
    //        {
    //            __result = CustomCargoInjector.IsCustomTypeRegistered(cargoType);
    //            if (__result) return false;
    //        }
    //        return true;
    //    }
    //}

    public static class EnumExtensions
    {
        public static GameObject ToTypePrefab(this BogieType bogie)
        {
            return ((TrainCarType)bogie).ToV2().prefab;
        }

        public static GameObject ToTypePrefab(this BufferType buffer)
        {
            return ((TrainCarType)buffer).ToV2().prefab;
        }

        public static bool IsFront(this CouplerDirection direction)
        {
            return direction == CouplerDirection.Front;
        }

        public static AudioMixerGroup ToInstance(this DVAudioMixerGroup group)
        {
            return AudioHelpers.GetMixerGroup(group);
        }
    }
}
