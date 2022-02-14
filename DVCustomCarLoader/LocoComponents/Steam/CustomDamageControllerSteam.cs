using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts;
using DV.ServicePenalty;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomDamageControllerSteam :
        DamageControllerCustomLoco<CustomLocoControllerSteam, CustomLocoSimEventsSteam, DamageConfigBasic>
    {
        public override IEnumerable<DebtTrackingInfo> GetDebtComponents()
        {
            return new[]
            {
                new DebtTrackingInfo(this, new DebtComponent(bodyDamage.EffectiveHealthPercentage100Notation, ResourceType.Car_DMG)),
                new DebtTrackingInfo(this, new DebtComponent(wheels.HealthPercentage100Notation, ResourceType.Wheels_DMG)),
            };
        }

        public override void ResetDebt(DebtComponent debt)
        {
            switch (debt.type)
            {
                case ResourceType.Car_DMG:
                    debt.ResetComponent(bodyDamage.EffectiveHealthPercentage100Notation);
                    break;

                case ResourceType.Wheels_DMG:
                    debt.ResetComponent(wheels.HealthPercentage100Notation);
                    break;
            }
        }

        public override void UpdateDebtValue(DebtComponent debt)
        {
            switch (debt.type)
            {
                case ResourceType.Car_DMG:
                    debt.UpdateEndValue(bodyDamage.EffectiveHealthPercentage100Notation);
                    break;

                case ResourceType.Wheels_DMG:
                    debt.UpdateEndValue(wheels.HealthPercentage100Notation);
                    break;
            }
        }


        public override IEnumerable<PitStopRefillable> GetPitStopParameters()
        {
            return new[]
            {
                new PitStopRefillable(this, ResourceType.Car_DMG, bodyDamage.EffectiveHealthPercentage100Notation, 100),
                new PitStopRefillable(this, ResourceType.Wheels_DMG, wheels.HealthPercentage100Notation, 100),
            };
        }

        public override float GetPitStopLevel(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Car_DMG:
                    return bodyDamage.EffectiveHealthPercentage100Notation;
                case ResourceType.Wheels_DMG:
                    return wheels.HealthPercentage100Notation;
                default:
                    Main.Warning("Tried to get pit stop value this loco damage ctrl doesn't have");
                    return 0;
            }
        }

        public override void ChangePitStopLevel(ResourceType type, float changeAmount)
        {
            switch (type)
            {
                case ResourceType.Car_DMG:
                    bodyDamage.RepairCarEffectivePercentage(changeAmount / 100f);
                    break;

                case ResourceType.Wheels_DMG:
                    wheels.RepairDamagePercentage(changeAmount / 100f);
                    break;

                default:
                    Main.Warning("Trying to refill/repair something that is not part of this loco");
                    break;
            }
        }
    }
}
