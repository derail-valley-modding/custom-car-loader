using DV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    [HarmonyPatch(typeof(CarVisitChecker), "VisitConnectedCars")]
    public static class VisitCheckerPatch
    {
        private const float RECENTLY_VISITED_TIME_THRESHOLD = 7200f;
        private const float COUNTDOWN_TIME_UNIT = 5f;

        public static bool Prefix(CarVisitChecker __instance, TrainCar ___car, CountdownTimer ___recentlyVisitedTimer)
        {
            if (!CarTypeInjector.IsInCustomRange(___car.carType))
            {
                return true;
            }

            Coupler toVisit;
            bool otherIsTender;

            if (CarTypes.IsSteamLocomotive(___car.carType))
            {
                toVisit = ___car.rearCoupler.coupledTo;
                otherIsTender = true;
            }
            else if (CarTypes.IsTender(___car.carType))
            {
                toVisit = ___car.frontCoupler.coupledTo;
                otherIsTender = false;
            }
            else
            {
                Main.Error("Trying to use car visit checker on non steam loco or tender car");
                UnityEngine.Object.Destroy(__instance);
                return false;
            }

            if (!toVisit)
            {
                return false;
            }

            TrainCar otherCar = toVisit.train;
            if (!otherCar || 
                (otherIsTender && !CarTypes.IsTender(otherCar.carType)) || 
                (!otherIsTender && !CarTypes.IsSteamLocomotive(otherCar.carType)))
            {
                return false;
            }

            CarVisitChecker otherVisitChecker = otherCar.GetComponent<CarVisitChecker>();
            if (!otherVisitChecker)
            {
                Main.Error($"Couldn't get visit checker for connected car {otherCar.ID} type {otherCar.carType.DisplayName()}");
                return false;
            }

            ___recentlyVisitedTimer.StartCountdown(RECENTLY_VISITED_TIME_THRESHOLD, COUNTDOWN_TIME_UNIT);
            return false;
        }
    }
}
