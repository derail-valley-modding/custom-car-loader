using CCL.Importer.Components.Simulation;
using LocoSim.Implementations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CCL.Importer.Implementations
{
    internal class DieselBatteryHybrid : SimComponent
    {
        private readonly FuseReference powerFuseRef;

        private readonly PortReference throttle;
        private readonly PortReference generatorVoltage;
        private readonly PortReference voltageRegulatorVoltage;
        private readonly PortReference batteryVoltage;
        private readonly PortReference batteryChargeNorm;
        private readonly PortReference singleMotorEffectiveResistance;
        private readonly PortReference effectiveResistance;
        private readonly PortReference genGoalPower;
        private readonly PortReference genPowerOutRead;
        private readonly PortReference tmTotalAmps;
        private readonly PortReference tmTransitionCurrentLimit;
        private readonly PortReference tmPowerInRead;
        private readonly PortReference chargeDisableOverride;
        private readonly PortReference manualChargeToggle;
        private readonly PortReference chargeLimitUpperBound;
        private readonly PortReference chargeLimitLowerBound;
        private readonly PortReference hybridMode;
        private readonly PortReference extAmpLimitGen;
        private readonly PortReference extAmpLimitVoltReg;

        private readonly Port extCurrentLimitExtIn;
        private readonly Port throttleDeOut;
        private readonly Port throttleBatOut;
        private readonly Port smerGenOut;
        private readonly Port smerBatOut;
        private readonly Port effectiveResistanceOut;
        private readonly Port tmPowerInReadOut;
        private readonly Port voltageBusOut;
        private readonly Port genGoalPowerOut;
        private readonly Port tmTotalAmpsOut;
        private readonly Port tmTransitionCurrentLimitOut;
        private readonly Port chargingActive;
        private readonly Port activeMode;
        private readonly Port dieselModeState;
        private readonly Port batteryModeState;
        private readonly Port batteryLowState;

        private readonly float modeTransitionDelay;
        private readonly float maxChargeBatteryLevel;
        private readonly float startChargeBatteryLevel;
        private readonly float emergencyBatteryLevel;
        private readonly float rechargeRate;
        private readonly float chargingThrottle;
        private readonly float adaptiveChargingMaxPower;
        private readonly bool manualChargingOnly;
        private readonly bool adaptiveCharging;
        private readonly bool adjustableChargeHysteresis;

        private HybridMode selectorMode;
        private HybridMode requestedMode;
        private HybridMode lastRequestedMode;
        private HybridMode currentMode;
        private float transitionTimer;
        private bool isTransitioning;
        private bool withinChargeRange;
        private bool chargeOverride;
        private bool chargeDisable;
        private float chargingTarget;
        private bool batteryLow;
        private bool isCharging;
        private bool batteryFull;
        private bool isInBatteryMode;
        private float stopChargeThreshold;
        private float startChargeThreshold;
        private float extAmpLimitFloatIn;

        public override bool HasSaveData => true;
        private const string SOC_HYSTERESIS_STATE_SAVE_KEY = "socHysteresisState";
        private const string HYBRID_MODE_SAVE_KEY = "savedHybridMode";

        private enum HybridMode
        {
            Transition = -1,
            Diesel = 0,
            Battery = 1
        }

        public DieselBatteryHybrid(DieselBatteryHybridDefinitionInternal def) : base(def.ID)
        {
            modeTransitionDelay = def.modeTransitionDelay;
            maxChargeBatteryLevel = def.maxChargeBatteryLevel;
            startChargeBatteryLevel = def.startChargeBatteryLevel;
            emergencyBatteryLevel = def.emergencyBatteryLevel;
            rechargeRate = def.batteryRechargeRate;
            chargingThrottle = def.engineRechargeThrottle;
            adaptiveChargingMaxPower = def.adaptiveChargingMaxPower;
            manualChargingOnly = def.manualChargingOnly;
            adaptiveCharging = def.adaptiveCharging;
            adjustableChargeHysteresis = def.adjustableChargeHysteresis;

            powerFuseRef = AddFuseReference(def.powerFuseId);

            throttle = AddPortReference(def.throttle);
            generatorVoltage = AddPortReference(def.generatorVoltage);
            voltageRegulatorVoltage = AddPortReference(def.voltRegVoltage);
            batteryVoltage = AddPortReference(def.batteryVoltage);
            batteryChargeNorm = AddPortReference(def.batteryChargeNorm);
            singleMotorEffectiveResistance = AddPortReference(def.singleMotorEffectiveResistance);
            effectiveResistance = AddPortReference(def.effectiveResistance);
            genGoalPower = AddPortReference(def.genGoalPower);
            genPowerOutRead = AddPortReference(def.genPowerOutRead);
            tmTotalAmps = AddPortReference(def.tmTotalAmps);
            tmTransitionCurrentLimit = AddPortReference(def.transitionCurrentLimit);
            tmPowerInRead = AddPortReference(def.tmPowerIn);
            chargeDisableOverride = AddPortReference(def.chargeDisableOverride);
            manualChargeToggle = AddPortReference(def.manualChargeToggle);
            chargeLimitUpperBound = AddPortReference(def.chargeLimitUpperBound);
            chargeLimitLowerBound = AddPortReference(def.chargeLimitLowerBound);
            hybridMode = AddPortReference(def.hybridModeControl);
            extAmpLimitGen = AddPortReference(def.extAmpLimitGen);
            extAmpLimitVoltReg = AddPortReference(def.extAmpLimitVoltReg);

            extCurrentLimitExtIn = AddPort(def.extCurrentLimitExtIn);
            throttleDeOut = AddPort(def.throttleDeOut);
            throttleBatOut = AddPort(def.throttleBatteryOut);
            smerGenOut = AddPort(def.SmerGen);
            smerBatOut = AddPort(def.SmerBatt);
            effectiveResistanceOut = AddPort(def.effectiveResistanceOut);
            tmPowerInReadOut = AddPort(def.tmPowerInReadOut);
            voltageBusOut = AddPort(def.voltageBusOut);
            genGoalPowerOut = AddPort(def.genGoalPowerOut);
            tmTotalAmpsOut = AddPort(def.tmTotalAmpsOut);
            tmTransitionCurrentLimitOut = AddPort(def.transitionCurrentLimitOut);
            chargingActive = AddPort(def.chargingActive);
            activeMode = AddPort(def.activeMode);
            dieselModeState = AddPort(def.dieselModeState);
            batteryModeState = AddPort(def.batteryModeState);
            batteryLowState = AddPort(def.batteryLow);

            withinChargeRange = false;
            batteryFull = false;
            isInBatteryMode = false;
            batteryLow = false;
            isCharging = false;
        }

        public override void Tick(float delta)
        {
            // Sanitize limiter
            extAmpLimitFloatIn = extCurrentLimitExtIn.Value;
            if (extAmpLimitFloatIn == 0) extAmpLimitFloatIn = float.PositiveInfinity;

            // Mode selector
            if (hybridMode.Value < 0.5)
            {
                selectorMode = HybridMode.Diesel;
            }

            else if (hybridMode.Value >= 0.5)
            {
                selectorMode = HybridMode.Battery;
            }

            // No switching without power
            if (!powerFuseRef.State && powerFuseRef != null)
            {
                return;
            }

            SimulateSwitching(delta);
            HandleHysteresisThresholds();

            if (!isTransitioning || transitionTimer <= 0f)
            {
                SimulateModeConfigurations();
            }

            if (isCharging) chargingActive.Value = 1;
            else chargingActive.Value = 0;

            if (batteryLow) batteryLowState.Value = 1;
            else batteryLowState.Value = 0;
        }

        private void SimulateSwitching(float delta)
        {
            // Force diesel when low battery, disable switching
            if (batteryChargeNorm.Value <= emergencyBatteryLevel)
            {
                requestedMode = HybridMode.Diesel;
                batteryLow = true;
            }
            else
            {
                requestedMode = selectorMode;
                batteryLow = false;
            }

            // Keep resetting timer when selector is changed
            if (requestedMode != lastRequestedMode)
            {
                lastRequestedMode = requestedMode;
                isTransitioning = true;
                transitionTimer = modeTransitionDelay;
            }

            // Transition delay
            if (isTransitioning)
            {
                transitionTimer -= delta;

                if (transitionTimer > 0f)
                {
                    ConfigureDisconnectAll();

                    // Mode to indicate switching
                    activeMode.Value = (int)HybridMode.Transition;

                    // Do not proceed to current modes while transitioning
                    return;
                }

                // Transition finished
                transitionTimer = 0f;
                isTransitioning = false;

                currentMode = requestedMode;
            }

            activeMode.Value = (int)currentMode;
        }

        private void SimulateModeConfigurations()
        {
            // Lamps: 0 off, 1 on, 2 blink
            switch (currentMode)
            {
                case HybridMode.Diesel:
                    {
                        ConfigureDieselMode();

                        dieselModeState.Value = 1;
                        batteryModeState.Value = 0;

                        if (batteryLow && selectorMode == HybridMode.Battery)
                        {
                            dieselModeState.Value = 2;
                            batteryModeState.Value = 0;
                        }

                        break;
                    }

                case HybridMode.Battery:
                    {
                        ConfigureBatteryMode();

                        dieselModeState.Value = 0;
                        batteryModeState.Value = 1;

                        break;
                    }
            }
        }

        private void HandleHysteresisThresholds()
        {
            if (!adjustableChargeHysteresis)
            {
                startChargeThreshold = startChargeBatteryLevel;
                stopChargeThreshold = maxChargeBatteryLevel;
                return;
            }

            const float gap = 0.05f;

            float minLower = emergencyBatteryLevel + gap;
            float maxUpper = maxChargeBatteryLevel;

            float lower = Mathf.Lerp(emergencyBatteryLevel, maxChargeBatteryLevel, chargeLimitLowerBound.Value);
            float upper = Mathf.Lerp(emergencyBatteryLevel, maxChargeBatteryLevel, chargeLimitUpperBound.Value);

            float lowerClamped = Mathf.Clamp(lower, minLower, maxUpper - gap);
            float upperClamped = Mathf.Clamp(upper, lowerClamped + gap, maxUpper);

            startChargeThreshold = lowerClamped;
            stopChargeThreshold = upperClamped;
        }

        private void CheckIfAllowedToCharge()
        {
            chargeDisable = chargeDisableOverride.Value >= 0.5f;
            batteryFull = batteryChargeNorm.Value >= stopChargeThreshold;
            isInBatteryMode = currentMode == HybridMode.Battery;
            chargeOverride = manualChargeToggle.Value >= 0.5f;

            if (!manualChargingOnly)
                ProcessHysteresis();
            else
                withinChargeRange = false;
        }

        private void ProcessHysteresis()
        {
            if (!withinChargeRange)
            {
                if (batteryChargeNorm.Value <= startChargeThreshold)
                {
                    withinChargeRange = true;
                }
            }
            else
            {
                if (batteryChargeNorm.Value >= stopChargeThreshold)
                {
                    withinChargeRange = false;
                }
            }
        }

        private void SimulateCharging()
        {
            if (!chargeDisable && !batteryFull && isInBatteryMode && (chargeOverride || withinChargeRange))
            {
                throttleDeOut.Value = chargingThrottle;

                float resistance = CalculateChargeResistance();

                smerGenOut.Value = resistance;
                effectiveResistanceOut.Value = resistance;

                genGoalPowerOut.Value = chargingTarget;

                tmPowerInReadOut.Value = CalculateChargePower();
                tmTotalAmpsOut.Value = CalculateChargeAmps(resistance);

                isCharging = true;
            }
            else
            {
                throttleDeOut.Value = 0f;

                smerGenOut.Value = float.PositiveInfinity;
                effectiveResistanceOut.Value = float.PositiveInfinity;
                genGoalPowerOut.Value = 0f;

                tmPowerInReadOut.Value = tmPowerInRead.Value;
                tmTotalAmpsOut.Value = 0f;

                isCharging = false;
            }
        }

        private void CalculateChargeTarget()
        {
            chargingTarget = adaptiveCharging ?
                Mathf.Clamp(tmPowerInRead.Value + rechargeRate, 0f, adaptiveChargingMaxPower) :
                rechargeRate;
        }

        private float CalculateChargeResistance()
        {
            float v = batteryVoltage.Value;
            float voltsSq = v * v;

            float resistance = voltsSq / chargingTarget;

            return resistance;
        }

        private float CalculateChargeAmps(float resistance)
        {
            if (resistance <= 0f || float.IsInfinity(resistance))
                return 0f;

            return generatorVoltage.Value / resistance;
        }

        private float CalculateChargePower()
        {
            float chargeIn = -genPowerOutRead.Value;
            float powerDraw = tmPowerInRead.Value;

            return powerDraw + chargeIn;
        }

        private void ConfigureDisconnectAll()
        {
            voltageBusOut.Value = 0f;

            throttleDeOut.Value = 0f;

            smerGenOut.Value = float.PositiveInfinity;
            effectiveResistanceOut.Value = float.PositiveInfinity;
            genGoalPowerOut.Value = 0f;

            tmTotalAmpsOut.Value = 0f;
            tmTransitionCurrentLimitOut.Value = float.PositiveInfinity;

            tmPowerInReadOut.Value = 0f;
            throttleBatOut.Value = 0f;
            smerBatOut.Value = float.PositiveInfinity;

            extAmpLimitGen.Value = float.PositiveInfinity;
            extAmpLimitVoltReg.Value = float.PositiveInfinity;

            isCharging = false;
        }

        private void ConfigureDieselMode()
        {
            voltageBusOut.Value = generatorVoltage.Value;

            throttleDeOut.Value = throttle.Value;

            smerGenOut.Value = singleMotorEffectiveResistance.Value;
            effectiveResistanceOut.Value = effectiveResistance.Value;
            genGoalPowerOut.Value = genGoalPower.Value;

            tmTotalAmpsOut.Value = tmTotalAmps.Value;
            tmTransitionCurrentLimitOut.Value = tmTransitionCurrentLimit.Value;

            tmPowerInReadOut.Value = 0f;
            throttleBatOut.Value = 0f;
            smerBatOut.Value = float.PositiveInfinity;

            extAmpLimitGen.Value = extAmpLimitFloatIn;
            extAmpLimitVoltReg.Value = extAmpLimitFloatIn;

            isCharging = false;
        }

        private void ConfigureBatteryMode()
        {
            voltageBusOut.Value = voltageRegulatorVoltage.Value;

            CalculateChargeTarget();
            CheckIfAllowedToCharge();
            SimulateCharging();

            tmTransitionCurrentLimitOut.Value = float.PositiveInfinity;

            throttleBatOut.Value = throttle.Value;
            smerBatOut.Value = singleMotorEffectiveResistance.Value;

            extAmpLimitGen.Value = float.PositiveInfinity;
            extAmpLimitVoltReg.Value = extAmpLimitFloatIn;
        }

        public override JObject GetSaveStateData()
        {
            JObject data = new JObject();
            data[SOC_HYSTERESIS_STATE_SAVE_KEY] = withinChargeRange;
            data[HYBRID_MODE_SAVE_KEY] = (int)currentMode;
            return data;
        }

        public override void SetSaveStateData(JObject savedData)
        {
            if (savedData.TryGetValue(SOC_HYSTERESIS_STATE_SAVE_KEY, out JToken token))
            {
                withinChargeRange = token.Value<bool>();
            }

            if (savedData.TryGetValue(HYBRID_MODE_SAVE_KEY, out JToken token2))
            {
                currentMode = lastRequestedMode = (HybridMode)token2.Value<int>();
            }
        }
    }
}