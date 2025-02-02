using CCL.Types;
using DV.ThingTypes;
using System.Collections;
using UnityEngine;

namespace CCL.Importer.Components
{
    internal class CarAutoCouplerInternal : MonoBehaviour
    {
        private const float AUTOCOUPLE_RANGE = 2.5f;
        private const float START_CHECKING_WAIT_TIME_LONG = 4f;
        private const float START_CHECKING_WAIT_TIME_SHORT = 1f;
        private const float CHECK_PERIOD = 2f;

        private Coupler _coupler = null!;
        private Coroutine? _checkAutoCoupleCoro;

        public CouplerDirection Direction;
        public CouplerDirection OtherDirection;
        public bool AlwaysCouple = false;
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];

        private IEnumerator Start()
        {
            var car = GetComponent<TrainCar>();
            _coupler = Direction.IsFront() ? car.frontCoupler : car.rearCoupler;

            if (!_coupler)
            {
                Debug.LogError("CarAutoCoupler couldn't find Coupler component during init", this);
                yield break;
            }

            yield return WaitFor.Seconds(1.5f);

            StartAutoCoupleCoroIfConditionsFulfilled(true);
            SetupListeners(true);
        }

        private void OnDisable()
        {
            if (UnloadWatcher.isUnloading) return;

            KillAutoCoupleCoro();
        }

        private void SetupListeners(bool on)
        {
            if (on)
            {
                _coupler.Coupled += OnCoupled;
                _coupler.Uncoupled += OnUncouple;
                _coupler.train.OnRerailed += OnRerailed;
                _coupler.train.OnDerailed += OnDerailed;
            }
            else
            {
                _coupler.Coupled -= OnCoupled;
                _coupler.Uncoupled -= OnUncouple;
                _coupler.train.OnRerailed -= OnRerailed;
                _coupler.train.OnDerailed -= OnDerailed;
            }
        }

        private IEnumerator CheckTenderAutoCouple(bool longStartWait)
        {
            yield return WaitFor.Seconds(longStartWait ? START_CHECKING_WAIT_TIME_LONG : START_CHECKING_WAIT_TIME_SHORT);

            while (true)
            {
                yield return WaitFor.Seconds(CHECK_PERIOD);

                if (_coupler.IsCoupled()) break;

                var firstCouplerInRange = _coupler.GetFirstCouplerInRange(AUTOCOUPLE_RANGE);
                if (firstCouplerInRange != null && !firstCouplerInRange.IsCoupled())
                {
                    var car = firstCouplerInRange.train;
                    var otherCoupler = OtherDirection.IsFront() ? car.frontCoupler : car.rearCoupler;
                    if (!car.derailed && MeetsConditions(car.carLivery) && otherCoupler == firstCouplerInRange)
                    {
                        _coupler.TryCouple(true, false, AUTOCOUPLE_RANGE);

                        if (_coupler.IsCoupled())
                        {
                            _checkAutoCoupleCoro = null;
                            yield break;
                        }

                        Debug.LogError("Unexpected state, failed couple attempt!", this);
                    }
                }
            }

            Debug.LogError("Unexpected state, coro shouldn't run if coupler is coupled!", this);
            _checkAutoCoupleCoro = null;
        }

        private void OnRerailed() => StartAutoCoupleCoroIfConditionsFulfilled(false);

        private void OnUncouple(object sender, UncoupleEventArgs e) => StartAutoCoupleCoroIfConditionsFulfilled(true);

        private void OnCoupled(object sender, CoupleEventArgs e) => KillAutoCoupleCoro();

        private void OnDerailed(TrainCar _) => KillAutoCoupleCoro();

        private void StartAutoCoupleCoroIfConditionsFulfilled(bool longStartWait)
        {
            KillAutoCoupleCoro();

            if (!_coupler.train.derailed && !_coupler.IsCoupled())
            {
                _checkAutoCoupleCoro = StartCoroutine(CheckTenderAutoCouple(longStartWait));
            }
        }

        private void KillAutoCoupleCoro()
        {
            if (_checkAutoCoupleCoro != null)
            {
                StopCoroutine(_checkAutoCoupleCoro);
                _checkAutoCoupleCoro = null;
            }
        }

        private bool MeetsConditions(TrainCarLivery car)
        {
            if (AlwaysCouple) return true;

            foreach (var item in CarKinds)
            {
                if (car.parentType.kind.id == item) return true;
            }

            foreach (var item in CarTypes)
            {
                if (car.parentType.id == item) return true;
            }

            foreach (var item in CarLiveries)
            {
                if (car.id == item) return true;
            }

            return false;
        }
    }
}
