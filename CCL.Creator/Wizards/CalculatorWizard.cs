using CCL.Creator.Utility;
using CCL.Types;
using System;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class CalculatorWizard : EditorWindow
    {
        [MenuItem("CCL/Calculator", priority = MenuOrdering.MenuBar.Calculator)]
        public static void ShowWindow()
        {
            var window = GetWindow<CalculatorWizard>();
            window.Show();
            window.titleContent = new GUIContent("CCL - Calculator");
        }

        private static readonly GUIContent[] s_modes = new[]
        {
            new GUIContent("Speed/RPM",
                "Convert between speed and RPM"),
            new GUIContent("Power/Force",
                "Convert between power and force"),
            new GUIContent("Steam Tractive Effort",
                "Approximate tractive effort for steam locomotives"),
            new GUIContent("Adhesion Limit",
                "Adhesion limit")
        };

        private SerializedObject _editor = null!;
        private float _scroll = 0.0f;
        [SerializeField]
        private int _mode = -1;

        private void OnEnable()
        {
            _editor = new SerializedObject(this);
        }

        private void OnGUI()
        {
            _editor.Update();

            _scroll = EditorGUILayout.BeginScrollView(new Vector2(0, _scroll)).y;

            using (new EditorGUI.IndentLevelScope())
            {
                int modesPerRow = Mathf.Max(1, Mathf.CeilToInt(position.width / (s_modes.Length * 120.0f)));
                _mode = GUILayout.SelectionGrid(_mode, s_modes, modesPerRow, EditorStyles.miniButtonMid, GUILayout.ExpandWidth(true));
                EditorGUILayout.Space();

                switch (_mode)
                {
                    case 0:
                        _speedRpm.Draw();
                        break;
                    case 1:
                        _powerForce.Draw();
                        break;
                    case 2:
                        _steamTE.Draw();
                        break;
                    case 3:
                        _adhesionLimit.Draw();
                        break;
                    default:
                        EditorGUILayout.HelpBox("Please select an option above!", MessageType.Info);
                        break;
                }
            }

            EditorGUILayout.EndScrollView();
            _editor.ApplyModifiedProperties();
        }

        #region Speed/RPM

        [Serializable]
        private class SpeedRPM
        {
            public float WheelRadius = 0.459f;
            public float Speed;
            public float RPM = 0.0f;
            public float GearRatio = 1.0f;

            public void Draw()
            {
                EditorGUILayout.HelpBox("Fill in 3 values to calculate the 4th", MessageType.Info);

                WheelRadius = EditorGUILayout.FloatField("Wheel Radius (m)", WheelRadius);
                float circumference = MathHelper.Tau * WheelRadius;
                float speedMS = Speed * Units.KMHtoMS;

                if (GUILayout.Button("Calculate Radius"))
                {
                    circumference = speedMS * 60.0f / (RPM / GearRatio);
                    WheelRadius = circumference / MathHelper.Tau;
                }

                Speed = EditorGUILayout.FloatField("Speed (km/h)", Speed);
                speedMS = Speed * Units.KMHtoMS;

                if (GUILayout.Button("Calculate Speed"))
                {
                    speedMS = RPM / GearRatio * circumference / 60.0f;
                    Speed = speedMS * Units.MSToKMH;
                }

                RPM = EditorGUILayout.FloatField("RPM", RPM);

                if (GUILayout.Button("Calculate RPM"))
                {
                    RPM = speedMS * 60.0f / circumference * GearRatio;
                }

                GearRatio = EditorGUILayout.FloatField("Gear Ratio", GearRatio);

                if (GUILayout.Button("Calculate Gear Ratio"))
                {
                    GearRatio = speedMS * 60.0f / circumference * (1.0f / RPM);
                    GearRatio = 1.0f / GearRatio;
                }
            }
        }

        [SerializeField]
        private SpeedRPM _speedRpm = new SpeedRPM();

        #endregion

        #region Power/Force

        [Serializable]
        private class PowerForce
        {
            public float Power;
            public MetricPrefix PowerPrefix = MetricPrefix.Kilo;
            public float Force;
            public MetricPrefix ForcePrefix = MetricPrefix.Kilo;
            public float Speed;

            public void Draw()
            {
                EditorGUILayout.HelpBox("Fill in 2 values to calculate the 3rd", MessageType.Info);

                float speedMS = Speed * Units.KMHtoMS;

                Power = EditorHelpers.FloatFieldWithPrefix("Power (W)", Power, ref PowerPrefix);

                if (GUILayout.Button("Calculate Power"))
                {
                    Power = Force * speedMS;
                }

                Force = EditorHelpers.FloatFieldWithPrefix("Force (N)", Force, ref ForcePrefix);

                if (GUILayout.Button("Calculate Force"))
                {
                    Force = Power / speedMS;
                }

                Speed = EditorGUILayout.FloatField("Speed (km/h)", Speed);

                if (GUILayout.Button("Calculate Speed"))
                {
                    speedMS = Power / Force;
                    Speed = speedMS * Units.MSToKMH;
                }
            }
        }

        [SerializeField]
        private PowerForce _powerForce = new PowerForce();

        #endregion

        #region Steam TE

        [Serializable]
        private class SteamTE
        {
            public float Pressure = 14;
            public float CylinderBore = 0.61f;
            public float PistonStroke = 0.711f;
            public float DriverRadius = 0.712f;
            public float Cutoff = 85.0f;
            public int CylCount = 2;

            public void Draw()
            {
                Pressure = EditorGUILayout.FloatField("Boiler Pressure (bar)", Pressure);
                CylinderBore = EditorGUILayout.FloatField("Cylinder Bore (m)", CylinderBore);
                PistonStroke = EditorGUILayout.FloatField("Piston Stroke (m)", PistonStroke);
                DriverRadius = EditorGUILayout.FloatField("Driver Radius (m)", DriverRadius);
                Cutoff = EditorGUILayout.Slider("Cutoff (%)", Cutoff, 0, 100);
                CylCount = Mathf.Max(1, EditorGUILayout.IntField("Number Of Cylinders", CylCount));

                var result = Cutoff * CylCount * Pressure * CylinderBore * CylinderBore * PistonStroke / (DriverRadius * 400.0f);

                EditorGUILayout.LabelField("Tractive Effort", $"{result * Units.BarToPascal:F0} N");

                // TODO: Compounding?
            }
        }

        [SerializeField]
        private SteamTE _steamTE = new SteamTE();

        #endregion

        #region Adhesion Limit

        [Serializable]
        private class AdhesionLimit
        {
            private float TotalWeight = 38200;
            private float FrictionCoeff = 0.2f;
            private int TotalAxles = 2;
            private int PoweredAxles = 2;

            public void Draw()
            {
                TotalWeight = EditorGUILayout.FloatField("Total Weight (kg)", TotalWeight);
                FrictionCoeff = EditorGUILayout.FloatField("Friction Coefficient", FrictionCoeff);
                TotalAxles = EditorGUILayout.IntField("Total Axles", TotalAxles);
                PoweredAxles = EditorGUILayout.IntField("Powered Axles", PoweredAxles);

                var result = TotalWeight * Units.Gravity / TotalAxles * PoweredAxles * FrictionCoeff;
                EditorGUILayout.LabelField("Adhesion Limit", $"{result:F0} N");
            }
        }

        [SerializeField]
        private AdhesionLimit _adhesionLimit = new AdhesionLimit();

        #endregion
    }
}
