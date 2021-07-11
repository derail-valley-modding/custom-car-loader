using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DV.ServicePenalty;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class DamageControllerCustomDiesel : DamageControllerCustomLoco
    {
        [Header("Additional damage types")]
        public TrainDamage engine;

        #region Debt Tracking

        public override IEnumerable<DebtTrackingInfo> GetDebtComponents()
        {
            return new[]
            {
                new DebtTrackingInfo(this, new DebtComponent(bodyDamage.EffectiveHealthPercentage100Notation, ResourceType.Car_DMG)),
                new DebtTrackingInfo(this, new DebtComponent(wheels.HealthPercentage100Notation, ResourceType.Wheels_DMG)),
                new DebtTrackingInfo(this, new DebtComponent(engine.HealthPercentage100Notation, ResourceType.Engine_DMG))
            };
        }

        public override void ResetDebt( DebtComponent debt )
        {
            switch( debt.type )
            {
                case ResourceType.Car_DMG:
                    debt.ResetComponent(bodyDamage.EffectiveHealthPercentage100Notation);
                    break;

                case ResourceType.Wheels_DMG:
                    debt.ResetComponent(wheels.HealthPercentage100Notation);
                    break;

                case ResourceType.Engine_DMG:
                    debt.ResetComponent(engine.HealthPercentage100Notation);
                    break;
            }
        }

        public override void UpdateDebtValue( DebtComponent debt )
        {
            switch( debt.type )
            {
                case ResourceType.Car_DMG:
                    debt.UpdateEndValue(bodyDamage.EffectiveHealthPercentage100Notation);
                    break;

                case ResourceType.Wheels_DMG:
                    debt.UpdateEndValue(wheels.HealthPercentage100Notation);
                    break;

                case ResourceType.Engine_DMG:
                    debt.UpdateEndValue(engine.HealthPercentage100Notation);
                    break;
            }
        }

        #endregion

        #region Pit Stop Parameters

        public override IEnumerable<PitStopRefillable> GetPitStopParameters()
        {
            return new[]
            {
                new PitStopRefillable(this, ResourceType.Engine_DMG, engine.HealthPercentage100Notation, 100),
                new PitStopRefillable(this, ResourceType.Car_DMG, bodyDamage.EffectiveHealthPercentage100Notation, 100),
                new PitStopRefillable(this, ResourceType.Wheels_DMG, wheels.HealthPercentage100Notation, 100),
            };
        }

        public override float GetPitStopLevel( ResourceType type )
        {
            switch( type )
            {
                case ResourceType.Car_DMG:
                    return bodyDamage.EffectiveHealthPercentage100Notation;
                case ResourceType.Wheels_DMG:
                    return wheels.HealthPercentage100Notation;
                case ResourceType.Engine_DMG:
                    return engine.HealthPercentage100Notation;
                default:
                    Main.Warning("Tried to get pit stop value this loco damage ctrl doesn't have");
                    return 0;
            }
        }

        public override void ChangePitStopLevel( ResourceType type, float changeAmount )
        {
            switch( type )
            {
                case ResourceType.Car_DMG:
                    bodyDamage.RepairCarEffectivePercentage(changeAmount / 100f);
                    //if( bodyDamage.DamagePercentage < 0.05f )
                    //{
                    //    // Repair windows
                    //}
                    break;

                case ResourceType.Wheels_DMG:
                    wheels.RepairDamagePercentage(changeAmount / 100f);
                    break;

                case ResourceType.Engine_DMG:
                    engine.RepairDamagePercentage(changeAmount / 100f);
                    break;

                default:
                    Main.Warning("Trying to refill/repair something that is not part of this loco");
                    break;
            }
        }

        #endregion
    }
}