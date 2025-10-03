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

        private static GUIContent[] s_modes = new[]
        {
            new GUIContent("Speed/RPM",
                "Convert between speed and RPM"),
            new GUIContent("Power/Force",
                "Convert between power and force"),
            new GUIContent("Steam Tractive Effort",
                "Approximate tractive effort for steam locomotives")
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
                        DrawSpeedRPM();
                        break;
                    case 1:
                        DrawPowerForce();
                        break;
                    case 2:
                        DrawSteamTE();
                        break;
                    default:
                        EditorGUILayout.HelpBox("Please select an option above!", MessageType.Info);
                        break;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        #region Speed/RPM

        private float _wheelRadius = 0.459f;
        private float _speed;
        private float _rpm = 0.0f;
        private float _gearRatio = 1.0f;

        private void DrawSpeedRPM()
        {
            EditorGUILayout.HelpBox("Fill in 3 values to calculate the 4th", MessageType.Info);

            float circumference = 2.0f * Mathf.PI * _wheelRadius;
            float speedMS = _speed / 3.6f;

            _wheelRadius = EditorGUILayout.FloatField("Wheel Radius (m)", _wheelRadius);
            circumference = 2.0f * Mathf.PI * _wheelRadius;

            if (GUILayout.Button("Calculate Radius"))
            {
                circumference = speedMS * 60.0f / (_rpm / _gearRatio);
                _wheelRadius = circumference / (2.0f  * Mathf.PI);
            }

            _speed = EditorGUILayout.FloatField("Speed (km/h)", _speed);
            speedMS = _speed / 3.6f;

            if (GUILayout.Button("Calculate Speed"))
            {
                speedMS = _rpm / _gearRatio * circumference / 60.0f;
                _speed = speedMS * 3.6f;
            }

            _rpm = EditorGUILayout.FloatField("RPM", _rpm);

            if (GUILayout.Button("Calculate RPM"))
            {
                _rpm = speedMS * 60.0f / circumference * _gearRatio;
            }

            _gearRatio = EditorGUILayout.FloatField("Gear Ratio", _gearRatio);

            if (GUILayout.Button("Calculate Gear Ratio"))
            {
                _gearRatio = speedMS * 60.0f / circumference * (1.0f / _rpm);
                _gearRatio = 1.0f / _gearRatio;
            }
        }

        #endregion

        #region Power/Force

        private float _power;
        private float _force;

        private void DrawPowerForce()
        {
            EditorGUILayout.HelpBox("Fill in 2 values to calculate the 3rd", MessageType.Info);

            float speedMS = _speed / 3.6f;

            _power = EditorGUILayout.FloatField("Power (W)", _power);

            if (GUILayout.Button("Calculate Power"))
            {
                _power = _force * speedMS;
            }

            _force = EditorGUILayout.FloatField("Force (N)", _force);

            if (GUILayout.Button("Calculate Force"))
            {
                _force = _power / speedMS;
            }

            _speed = EditorGUILayout.FloatField("Speed (km/h)", _speed);

            if (GUILayout.Button("Calculate Speed"))
            {
                speedMS = _power / _force;
                _speed = speedMS * 3.6f;
            }
        }

        #endregion

        #region Steam TE

        private float _pressure = 14;
        private float _cylinderBore = 0.61f;
        private float _pistonStroke = 0.711f;
        private float _driverRadius = 0.712f;
        private int _cylCount = 2;

        private void DrawSteamTE()
        {
            _pressure = EditorGUILayout.FloatField("Boiler Pressure (bar)", _pressure);
            _cylinderBore = EditorGUILayout.FloatField("Cylinder Bore (m)", _cylinderBore);
            _pistonStroke = EditorGUILayout.FloatField("Piston Stroke (m)", _pistonStroke);
            _driverRadius = EditorGUILayout.FloatField("Driver Radius (m)", _driverRadius);
            _cylCount = Mathf.Max(1, EditorGUILayout.IntField("Number Of Cylinders", _cylCount));

            float result = 0.85f * _cylCount * _pressure * _cylinderBore * _cylinderBore * _pistonStroke / (_driverRadius * 4.0f);

            EditorGUILayout.LabelField("Tractive Effort", $"{result * 100000.0f:F0} N");
        }

        #endregion
    }
}
