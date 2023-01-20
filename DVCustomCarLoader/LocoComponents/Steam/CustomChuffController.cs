using DVCustomCarLoader.Effects;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomChuffController : MonoBehaviour
    {
        public event ChuffController.ChuffAction OnChuff;

        protected CustomLocoControllerSteam loco;
        protected DrivingAnimation driverAnimation;

        protected float wheelCircumference;
        public float chuffsPerRevolution = 2;

        public int currentChuff { get; protected set; }
        public int lastChuff { get; protected set; }
        public float chuffKmh;
        public float chuffPower;

        /// <summary>Current percentage of revolution (0.0 - 1.0)</summary>
        public float revolutionPos = 0;
        /// <summary>Accumulated # of revolutions for chuff</summary>
        public float revolutionAccumulator = 0;

        private const float MPS_KPH_FACTOR = 3.6f;

        protected void Awake()
        {
            loco = GetComponent<CustomLocoControllerSteam>();
            if (!loco)
            {
                Main.Error("Chuff Controller: No custom steam controller for chuff controller");
                enabled = false;
            }

            var simParams = GetComponent<CCL_GameScripts.SimParamsSteam>();
            chuffsPerRevolution = simParams.NumberOfCylinders * simParams.DriverGearRatio * 2;

            driverAnimation = GetComponents<DrivingAnimation>().Where(d => d.IsDrivingWheels).SingleOrDefault();
            if (!driverAnimation)
            {
                Main.Error("Chuff Controller: No driver animation found, or more than one driver animation found");
                enabled = false;
                return;
            }

            wheelCircumference = driverAnimation.DefaultWheelRadius * Mathf.PI * 2;
            Main.LogVerbose("CustomChuffController awakened");
        }

        protected void Update()
        {
            chuffPower = loco.GetTotalPowerForcePercentage();

            float delta = driverAnimation.defaultRotationSpeed * Time.deltaTime;

            revolutionPos = (revolutionPos + Mathf.Abs(delta)) % 1f;
            revolutionAccumulator += delta * chuffsPerRevolution;
            
            currentChuff = Mathf.FloorToInt(revolutionAccumulator);
            chuffKmh = Mathf.Abs(driverAnimation.defaultRotationSpeed * wheelCircumference * MPS_KPH_FACTOR);

            if (currentChuff != lastChuff)
            {
                lastChuff = currentChuff;
                OnChuff?.Invoke(chuffPower);
            }

            const float NORM_LIMIT = 10000;
            if (revolutionAccumulator > NORM_LIMIT)
            {
                revolutionAccumulator -= NORM_LIMIT;
            }
            else if (revolutionAccumulator < -NORM_LIMIT)
            {
                revolutionAccumulator += NORM_LIMIT;
            }
        }
    }
}