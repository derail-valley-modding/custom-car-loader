using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomTenderAutoCouple : MonoBehaviour
    {
		private const float AUTOCOUPLE_RANGE = 2.5f;
		private const float START_DELAY_LONG = 4f;
		private const float START_DELAY_SHORT = 1f;
		private const float CHECK_PERIOD = 2f;

		private ResourceType engineFuelType;
		private TrainCar attachedCar;
		private Coupler rearCoupler;
		private Coroutine checkAutoCoupleCoro;

		public IEnumerator Start()
        {
			attachedCar = GetComponent<TrainCar>();
			rearCoupler = attachedCar.rearCoupler;
			if (!rearCoupler)
            {
				Main.Error("Custom Tender Auto Couple couldn't find train coupler");
				yield break;
            }

			yield return WaitFor.Seconds(1.5f);

			var sim = GetComponent<CustomLocoSimSteam>();
			if (sim)
            {
				engineFuelType = sim.FuelType;
			}

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
				rearCoupler.Coupled += OnCoupled;
				rearCoupler.Uncoupled += OnUncouple;
				rearCoupler.train.OnRerailed += OnRerailed;
				rearCoupler.train.OnDerailed += OnDerailed;
			}
			else
			{
				rearCoupler.Coupled -= OnCoupled;
				rearCoupler.Uncoupled -= OnUncouple;
				rearCoupler.train.OnRerailed -= OnRerailed;
				rearCoupler.train.OnDerailed -= OnDerailed;
			}
		}

		private void TryStartAutoCoupleCheck(bool longDelay)
        {
			KillAutoCoupleCoro();
			if (!attachedCar.derailed && !rearCoupler.IsCoupled())
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

				if (rearCoupler.IsCoupled())
				{
					break;
				}

				Coupler firstCouplerInRange = rearCoupler.GetFirstCouplerInRange(AUTOCOUPLE_RANGE);
				if (firstCouplerInRange != null && !firstCouplerInRange.IsCoupled())
				{
					var carInRange = firstCouplerInRange.train;
					if (!carInRange.derailed && CarTypes.IsTender(carInRange.carType) && (carInRange.frontCoupler == firstCouplerInRange))
					{
						ResourceType tenderFuel;
						var customTender = carInRange.GetComponent<CustomTenderSimulation>();
						if (customTender) 
                        {
							tenderFuel = customTender.FuelType;
                        }
						else
                        {
							tenderFuel = ResourceType.Coal;
                        }

#if DEBUG
						Main.LogVerbose($"Trying to couple engine {attachedCar.ID} ({engineFuelType}) to tender {carInRange.ID} ({tenderFuel})");
#endif

						if (tenderFuel == engineFuelType)
						{
							rearCoupler.TryCouple(true, false, AUTOCOUPLE_RANGE);
							if (rearCoupler.IsCoupled())
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
			}

			Main.Error("Unexpected state, coro shouldn't run if rearCoupler is coupled!");
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
