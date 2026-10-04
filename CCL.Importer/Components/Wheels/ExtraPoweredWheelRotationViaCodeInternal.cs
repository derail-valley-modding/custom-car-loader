using DV.Wheels;
using UnityEngine;

namespace CCL.Importer.Components.Wheels
{
    internal class ExtraPoweredWheelRotationViaCodeInternal : PoweredWheelRotationBase
    {
        public void Update()
        {
            var delta = Time.deltaTime * 360;
            var rps = GetRPS();

            if (rps == 0f) return;

            float? rolling = null;

            foreach (var wheel in poweredWheelsManager.poweredWheels)
            {
                var final = rps;

                if (!wheel.IsPowered)
                {
                    rolling ??= GetRollingRPS();
                    final = rolling.Value;
                }

                final *= delta;
                wheel.wheelTransform.Rotate(wheel.localRotationAxis, final, Space.Self);

                if (wheel is ExtraPoweredWheelInternal extra)
                {
                    foreach (var item in extra.CoupledAxles)
                    {
                        item.Transform.Rotate(item.Axis, final * item.Multiplier, Space.Self);
                    }
                }
            }
        }
    }
}
