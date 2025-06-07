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

            if (GUILayout.Button("Reset Physics"))
            {
                car.bogieSuspensionMultiplier = 1.0f;
                car.rollingResistanceCoefficient = CustomCarType.ROLLING_RESISTANCE_COEFFICIENT;
                car.wheelSlidingFrictionCoefficient = CustomCarType.WHEELSLIDE_FRICTION_COEFFICIENT;
                car.wheelslipFrictionCoefficient = CustomCarType.WHEELSLIP_FRICTION_COEFFICIENT;
                flag = true;
            }

            if (flag)
            {
                AssetHelper.SaveAsset(car);
            }
        }
    }
}
