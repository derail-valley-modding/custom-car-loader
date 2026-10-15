using CCL.Creator.Utility;
using CCL.Types;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(CustomCarType)), CanEditMultipleObjects]
    internal class CustomCarTypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var car = (CustomCarType)target;
            bool flag = false;

            EditorHelpers.DrawHeader("Additional Info");
            EditorGUILayout.LabelField("Force Per Bogie", $"{car.brakes.ForcePerBogie}");
            EditorGUILayout.LabelField("Total Price", $"{car.damage.TotalPrice}");

            if (GUILayout.Button("Reset Physics"))
            {
                car.bogieSuspensionMultiplier = 1.0f;
                car.rollingResistanceCoefficient = CustomCarType.ROLLING_RESISTANCE_COEFFICIENT;
                car.wheelSlidingFrictionCoefficient = CustomCarType.WHEELSLIDE_FRICTION_COEFFICIENT;
                car.wheelslipFrictionCoefficient = CustomCarType.WHEELSLIP_FRICTION_COEFFICIENT;
                flag = true;
            }

            if (GUILayout.Button("Reset Damage"))
            {
                car.damage.ApplyWagonDefaults();
                flag = true;
            }

            if (GUILayout.Button("Open Wiki Resources"))
            {
                Application.OpenURL("https://github.com/derail-valley-modding/custom-car-loader/wiki/Other-Resources#damage-properties");
            }

            if (flag)
            {
                AssetHelper.SaveAsset(car);
            }
        }
    }
}
