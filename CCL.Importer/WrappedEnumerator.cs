using System;
using System.Collections;

namespace CCL.Importer
{
    public class WrappedEnumerator : IEnumerator
    {
        private readonly IEnumerator enumerator;
        private event Action? OnMoveNext;
        private event Action? OnComplete;

        public WrappedEnumerator(IEnumerator toWrap)
        {
            enumerator = toWrap;
        }

        public static WrappedEnumerator OnceCompleted(IEnumerator toWrap, Action onCompletion)
        {
            return new WrappedEnumerator(toWrap).WithOnComplete(onCompletion);
        }

        public static WrappedEnumerator AfterEach(IEnumerator toWrap, Action onMoveNext)
        {
            return new WrappedEnumerator(toWrap).WithMoveNext(onMoveNext);
        }

        public WrappedEnumerator WithMoveNext(Action onMoveNext)
        {
            OnMoveNext += onMoveNext;
            return this;
        }

        public WrappedEnumerator WithOnComplete(Action onComplete)
        {
            OnComplete += onComplete;
            return this;
        }

        public object Current => enumerator.Current;

        public bool MoveNext()
        {
            bool result = enumerator.MoveNext();

            OnMoveNext?.Invoke();
            if (!result)
            {
                OnComplete?.Invoke();
            }

            return result;
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }

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
}
