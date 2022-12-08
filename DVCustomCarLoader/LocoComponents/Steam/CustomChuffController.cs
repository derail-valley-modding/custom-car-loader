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
        public int chuffsPerRevolution = 2;

        public int currentChuff;
        private int lastChuff;
        public float chuffKmh;
        public float chuffPower;
        private float revolutionPos = 0;

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
            chuffsPerRevolution = simParams.ChuffsPerRevolution;

            driverAnimation = GetComponents<DrivingAnimation>().Where(d => d.IsDrivingWheels).SingleOrDefault();
            if (!driverAnimation)
            {
                Main.Error("Chuff Controller: No driver animation found, or more than one driver animation found");
                enabled = false;
            }

            wheelCircumference = driverAnimation.DefaultWheelRadius * Mathf.PI * 2;
            Main.LogVerbose("CustomChuffController awakened");
        }

        protected void Start()
        {
            
            Main.LogVerbose($"CustomChuffController: {chuffsPerRevolution} per rev");
        }

        protected void Update()
        {
            chuffPower = loco.GetTotalPowerForcePercentage();

            revolutionPos += driverAnimation.defaultRotationSpeed * Time.deltaTime;
            currentChuff = Mathf.FloorToInt(revolutionPos * chuffsPerRevolution);
            chuffKmh = Mathf.Abs(driverAnimation.defaultRotationSpeed * wheelCircumference * MPS_KPH_FACTOR);

            if (currentChuff != lastChuff)
            {
                lastChuff = currentChuff;
                OnChuff?.Invoke(chuffPower);
            }

            const float NORM_LIMIT = 10000;
            if (revolutionPos > NORM_LIMIT)
            {
                revolutionPos -= NORM_LIMIT;
            }
            else if (revolutionPos < -NORM_LIMIT)
            {
                revolutionPos += NORM_LIMIT;
            }
        }
    }
}