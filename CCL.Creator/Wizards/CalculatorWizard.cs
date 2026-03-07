using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Components.Simulation;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Electric;
using CCL.Types.Proxies.Simulation.Steam;
using System;
using UnityEditor;
using UnityEngine;

using UObject = UnityEngine.Object;

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
                "Adhesion limit"),
            new GUIContent("Traction Motor Properties",
                "Motor voltage and current for different configurations"),
            new GUIContent("Generator Voltage",
                "Expected maximum generator voltage")
        };

        private static readonly GUIContent s_context = new GUIContent("Context Object",
            "Drag an asset or component here to try and assign relevant information automatically");

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
                    case 4:
                        _tmProperties.Draw();
                        break;
                    case 5:
                        _generatorVoltage.Draw();
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
            public float Speed = 0.0f;
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
                    Speed = speedMS * Units.MStoKMH;
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

                EditorGUILayout.Space();
                Guess(EditorHelpers.ObjectField<UObject>(s_context, null, true));
            }

            private void Guess(UObject? context)
            {
                int index;

                switch (context)
                {
                    case CustomCarType car:
                        WheelRadius = car.wheelRadius;
                        break;
                    case CustomCarVariant livery:
                        Guess(livery.parentType);
                        Guess(livery.prefab);
                        break;

                    case TractionMotorSetDefinitionProxy tms:
                        RPM = tms.maxMotorRpm;
                        break;
                    case DieselEngineDirectDefinitionProxy engine:
                        RPM = RPM == engine.engineRpmMax ? engine.engineRpmIdle : engine.engineRpmMax;
                        break;
                    case RPMDamageCalculatorDefinition damage:
                        RPM = damage.MaxRPM;
                        break;

                    case TransmissionFixedGearDefinitionProxy transmission:
                        GearRatio = transmission.gearRatio;
                        break;
                    case SmoothTransmissionDefinitionProxy transmission:
                        if (transmission.gearRatios.Length == 0) break;
                        index = transmission.gearRatios.FirstIndexMatch(x => x == GearRatio);
                        GearRatio = transmission.gearRatios[(index + 1) % transmission.gearRatios.Length];
                        break;
                    case HydraulicTransmissionDefinitionProxy transmission:
                        if (transmission.configs.Length == 0) break;
                        index = transmission.configs.FirstIndexMatch(x => x.gearRatio == GearRatio);
                        GearRatio = transmission.configs[(index + 1) % transmission.configs.Length].gearRatio;
                        break;

                    case GameObject prefab:
                        Guess(prefab.GetComponentInChildren<DieselEngineDirectDefinitionProxy>());
                        Guess(prefab.GetComponentInChildren<TractionMotorSetDefinitionProxy>());
                        Guess(prefab.GetComponentInChildren<RPMDamageCalculatorDefinition>());
                        Guess(prefab.GetComponentInChildren<HydraulicTransmissionDefinitionProxy>());
                        Guess(prefab.GetComponentInChildren<SmoothTransmissionDefinitionProxy>());
                        Guess(prefab.GetComponentInChildren<TransmissionFixedGearDefinitionProxy>());
                        break;

                    default:
                        break;
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
                    Speed = speedMS * Units.MStoKMH;
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
                EditorGUILayout.LabelField("Expected Speed", $"{MathHelper.Tau * DriverRadius * 5.25 * Units.MStoKMH:F0} km/h");

                // TODO: Compounding?

                EditorGUILayout.Space();
                Guess(EditorHelpers.ObjectField<UObject>(s_context, null, true));
            }

            private void Guess(UObject? context)
            {
                switch (context)
                {
                    case CustomCarType car:
                        DriverRadius = car.wheelRadius;
                        break;
                    case CustomCarVariant livery:
                        Guess(livery.parentType);
                        Guess(livery.prefab);
                        break;

                    case BoilerDefinitionProxy boiler:
                        Pressure = boiler.safetyValveOpeningPressure;
                        break;

                    case ReciprocatingSteamEngineDefinitionProxy engine:
                        CylinderBore = engine.cylinderBore;
                        PistonStroke = engine.pistonStroke;
                        CylCount = engine.numCylinders;
                        break;

                    case GameObject prefab:
                        Guess(prefab.GetComponentInChildren<BoilerDefinitionProxy>());
                        Guess(prefab.GetComponentInChildren<ReciprocatingSteamEngineDefinitionProxy>());
                        break;

                    default:
                        break;
                }
            }
        }

        [SerializeField]
        private SteamTE _steamTE = new SteamTE();

        #endregion

        #region Adhesion Limit

        [Serializable]
        private class AdhesionLimit
        {
            public float TotalWeight = 38200;
            public float FrictionCoeff = 0.2f;
            public int TotalAxles = 2;
            public int PoweredAxles = 2;

            public void Draw()
            {
                TotalWeight = EditorGUILayout.FloatField("Total Weight (kg)", TotalWeight);
                FrictionCoeff = EditorGUILayout.FloatField("Friction Coefficient", FrictionCoeff);
                TotalAxles = EditorGUILayout.IntField("Total Axles", TotalAxles);
                PoweredAxles = EditorGUILayout.IntField("Powered Axles", PoweredAxles);

                var result = TotalWeight * Units.Gravity / TotalAxles * PoweredAxles * FrictionCoeff;
                EditorGUILayout.LabelField("Adhesion Limit", $"{result:F0} N");

                EditorGUILayout.Space();
                Guess(EditorHelpers.ObjectField<UObject>(s_context, null, true));
            }

            private void Guess(UObject? context)
            {
                switch (context)
                {
                    case CustomCarType car:
                        TotalWeight = car.mass;
                        FrictionCoeff = car.wheelslipFrictionCoefficient;
                        break;
                    case CustomCarVariant livery:
                        Guess(livery.parentType);
                        Guess(livery.prefab);
                        break;

                    case TractionMotorSetDefinitionProxy tms:
                        PoweredAxles = tms.numberOfTractionMotors;
                        break;

                    default:
                        break;
                }
            }
        }

        [SerializeField]
        private AdhesionLimit _adhesionLimit = new AdhesionLimit();

        #endregion

        #region Traction Motor Properties

        [Serializable]
        private class TractionMotorProperties
        {
            public float GeneratorVoltage = 1000;
            public float MotorCurrent = 500;
            public TractionMotorSetDefinitionProxy? Definition;

            public void Draw()
            {
                GeneratorVoltage = EditorGUILayout.FloatField("Generator Voltage", GeneratorVoltage);
                MotorCurrent = EditorGUILayout.FloatField("Current Per Motor", MotorCurrent);
                Definition = EditorHelpers.ObjectField("TM Definition", Definition, true);

                if (Definition == null)
                {
                    EditorGUILayout.HelpBox("Please link your current traction motor configuration in the field above", MessageType.Info);
                    return;
                }


                for (int i = 0; i < Definition.configurations.Length; i++)
                {
                    EditorHelpers.DrawSeparator();
                    EditorGUILayout.LabelField($"Config {i}");
                    var config = Definition.configurations[i];
                    var current = MotorCurrent * config.motorGroups.Length;

                    int seriesCount = config.motorGroups.Length > 0 ? config.motorGroups[0].motorIndexes.Length : 0;

                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField("Parallel Groups", $"{config.motorGroups.Length}");
                        EditorGUILayout.LabelField("Excitation", $"{config.excitationMultiplier:P2}");
                        EditorGUILayout.LabelField("Total Current", $"{current:F2} A");

                        for (int j = 0; j < config.motorGroups.Length; j++)
                        {
                            EditorGUILayout.LabelField($"Group {j}");
                            var group = config.motorGroups[j];
                            var voltage = GeneratorVoltage / group.motorIndexes.Length * config.excitationMultiplier;

                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Motors In Series", $"{group.motorIndexes.Length}");
                                EditorGUILayout.LabelField("Voltage/Motor", $"{voltage:F2} V");

                                if (seriesCount != group.motorIndexes.Length)
                                {
                                    EditorGUILayout.LabelField("Group size is not defined correctly!",
                                        EditorHelpers.StyleWithTextColour(EditorHelpers.Colors.DELETE_ACTION, GUI.skin.label));
                                }
                            }
                        }
                    }
                }
            }
        }

        [SerializeField]
        private TractionMotorProperties _tmProperties = new TractionMotorProperties();

        #endregion

        #region Generator Voltage

        [Serializable]
        private class GeneratorVoltage
        {
            public TractionGeneratorDefinitionProxy? Definition;
            public float ShaftRPM = 900;
            public bool UseExternalValues = false;
            public float Current = 500;
            public float Resistance = 1.5f;

            public void Draw()
            {
                Definition = EditorHelpers.ObjectField("Generator Definition", Definition, true);
                ShaftRPM = EditorGUILayout.FloatField("Shaft RPM", ShaftRPM);
                UseExternalValues = EditorGUILayout.Toggle("Use External Values", UseExternalValues);

                using (new EditorGUI.DisabledGroupScope(!UseExternalValues))
                {
                    Current = EditorGUILayout.FloatField("Current", Current);
                    Resistance = EditorGUILayout.FloatField("Single Motor Effective Resistance", Resistance);
                }

                if (Definition == null)
                {
                    return;
                }

                var excitation = 1.0f;
                var rads = ShaftRPM * MathHelper.RPMtoRadS;

                if (UseExternalValues)
                {
                    var voltage = Mathf.Min(Definition.maxVoltage, Current * Resistance);
                    excitation = Mathf.Clamp01(voltage / rads / Definition.torqueFactor);
                }

                EditorGUILayout.LabelField("Maximum Voltage", $"{rads * excitation * Definition.torqueFactor:F2} V");
                EditorGUILayout.LabelField("Excitation", $"{excitation:P2}");
            }
        }

        [SerializeField]
        private GeneratorVoltage _generatorVoltage = new GeneratorVoltage();

        #endregion
    }
}
