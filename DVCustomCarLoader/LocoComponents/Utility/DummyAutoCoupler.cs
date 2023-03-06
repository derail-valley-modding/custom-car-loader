using CCL_GameScripts;
using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Utility
{
    public class DummyAutoCoupler : MonoBehaviour
    {
        private const float AUTOCOUPLE_RANGE = 2.5f;
        private const float START_DELAY_LONG = 4f;
        private const float START_DELAY_SHORT = 1f;
        private const float CHECK_PERIOD = 2f;

        private TrainCar attachedCar;
        private DummySegmentController controller;
        private Coupler watchCoupler;
        private Coroutine checkAutoCoupleCoro;

        public IEnumerator Start()
        {
            attachedCar = GetComponent<TrainCar>();
            controller = GetComponent<DummySegmentController>();
            watchCoupler = controller.AutoCoupleSide == CarDirection.Reverse ? attachedCar.rearCoupler : attachedCar.frontCoupler;
            
            if (!watchCoupler)
            {
                Main.Error("Dummy segment auto coupler couldn't find train coupler");
                yield break;
            }

            yield return WaitFor.Seconds(1.5f);

            TryStartAutoCoupleCheck(true);
            SetupListeners(true);
            yield break;
        }

        private void OnDisable()
        {
            if (UnloadWatcher.isUnloading)
            {
                return;
            }
            KillAutoCoupleCoro();
        }

        private void SetupListeners(bool enable)
        {
            if (enable)
            {
                watchCoupler.Coupled += OnCoupled;
                watchCoupler.Uncoupled += OnUncouple;
                watchCoupler.train.OnRerailed += OnRerailed;
                watchCoupler.train.OnDerailed += OnDerailed;
            }
            else
            {
                watchCoupler.Coupled -= OnCoupled;
                watchCoupler.Uncoupled -= OnUncouple;
                watchCoupler.train.OnRerailed -= OnRerailed;
                watchCoupler.train.OnDerailed -= OnDerailed;
            }
        }

        private void TryStartAutoCoupleCheck(bool longDelay)
        {
            KillAutoCoupleCoro();
            if (!attachedCar.derailed && !watchCoupler.IsCoupled())
            {
                checkAutoCoupleCoro = StartCoroutine(CheckTenderAutoCouple(longDelay));
            }
        }

        private void KillAutoCoupleCoro()
        {
            if (checkAutoCoupleCoro != null)
            {
                StopCoroutine(checkAutoCoupleCoro);
                checkAutoCoupleCoro = null;
            }
        }

        private IEnumerator CheckTenderAutoCouple(bool longStartWait)
        {
            yield return WaitFor.Seconds(longStartWait ? START_DELAY_LONG : START_DELAY_SHORT);

            while (true)
            {
                yield return WaitFor.Seconds(CHECK_PERIOD);

                if (watchCoupler.IsCoupled())
                {
                    break;
                }

                Coupler firstCouplerInRange = watchCoupler.GetFirstCouplerInRange(AUTOCOUPLE_RANGE);
                if (firstCouplerInRange != null && !firstCouplerInRange.IsCoupled())
                {
                    var carInRange = firstCouplerInRange.train;
                    if (!carInRange.derailed && controller.ConnectsToCar(carInRange.carType))
                    {
#if DEBUG
                        Main.LogVerbose($"Trying to connect dummy segment {attachedCar.ID} to main unit {carInRange.ID}");
#endif
                        watchCoupler.TryCouple(true, false, AUTOCOUPLE_RANGE);
                        if (watchCoupler.IsCoupled())
                        {
#if DEBUG
                            Main.LogVerbose($"Auto-coupled steamer {attachedCar.ID} to tender {carInRange.ID}");
#endif
                            checkAutoCoupleCoro = null;
                            yield break;
                        }

                        Main.Error("Unexpected state, failed couple attempt!");
                    }
                }
            }

            Main.Error("Unexpected state, coro shouldn't run if coupled!");
            checkAutoCoupleCoro = null;
            yield break;
        }

        #region Events

        private void OnRerailed()
        {
            TryStartAutoCoupleCheck(false);
        }

        private void OnUncouple(object sender, UncoupleEventArgs e)
        {
            TryStartAutoCoupleCheck(true);
        }

        private void OnCoupled(object sender, CoupleEventArgs e)
        {
            KillAutoCoupleCoro();
        }

        private void OnDerailed(TrainCar _)
        {
            KillAutoCoupleCoro();
        }

        #endregion
    }
}
