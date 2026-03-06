using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class NamingWizard : EditorWindow
    {
        private enum Powertrain
        {
            DE,
            DH,
            DM,
            S,
            WE,
            BE,
            H,
            Other
        }

        private static readonly string[] s_powertrainOptions = new[]
        {
            "Diesel-Electric",
            "Diesel-Hydraulic",
            "Diesel-Mechanical",
            "Steam",
            "Wire-Electric",
            "Battery-Electric",
            "Hand",
            "Other"
        };

        private static readonly string[] s_powerRatingOptions = new[]
        {
            "Draisine",
            "Railbus",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"
        };

        private static readonly string[] s_decadeOptions = new[]
        {
            "1900s or earlier",
            "1910s",
            "1920s",
            "1930s",
            "1940s",
            "1950s",
            "1960s",
            "1970s",
            "1980s",
            "1990s",
        };

        private static readonly string[] s_capacityOptions = new[]
        {
            "None",
            "Passengers",
            "Freight",
            "Utility"
        };

        private static readonly string[] s_muOptions = new[]
        {
            "None",
            "First Unit",
            "Second Unit",
            "Third Unit",
            "Fourth Unit",
            "Fifth Unit",
            "Sixth Unit",
            "Seventh Unit",
            "Eighth Unit"
        };

        [MenuItem("CCL/Naming Wizard", priority = MenuOrdering.MenuBar.Naming)]
        public static void ShowWindow()
        {
            var window = GetWindow<NamingWizard>();
            window.Show();
            window.titleContent = new GUIContent("CCL - Naming Wizard");
        }

        private float _scroll = 0.0f;

        private Powertrain _powertrain;
        private string _otherPowertrain = string.Empty;
        private int _poweredAxles;
        private int _totalAxles;
        private string _whyteInput = string.Empty;
        private string _filteredWhyte = string.Empty;
        private int _capacity = 0;
        private int _powerRating = 3;
        private int _decade = 6;
        private int _unique = 0;
        private bool _isSlug = false;
        private int _multipleWorking = 0;

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(new Vector2(0, _scroll)).y;

            using (new EditorGUI.IndentLevelScope())
            {
                _powertrain = (Powertrain)EditorGUILayout.Popup("Powertrain", (int)_powertrain, s_powertrainOptions);

                EditorGUILayout.Space();

                switch (_powertrain)
                {
                    case Powertrain.S:
                        EditorGUILayout.HelpBox("DV uses a simplified Whyte notation for steam locomotives", MessageType.Info);
                        EditorGUI.BeginChangeCheck();
                        _whyteInput = EditorGUILayout.TextField("Whyte Notation", _whyteInput);

                        if (EditorGUI.EndChangeCheck())
                        {
                            _filteredWhyte = new string(_whyteInput.Where(char.IsDigit).ToArray());
                        }
                        break;
                    case Powertrain.Other:
                        _otherPowertrain = EditorGUILayout.TextField("Custom Powertrain", _otherPowertrain);
                        goto default;
                    default:
                        _totalAxles = Mathf.Max(1, EditorGUILayout.IntField("Total Axles", _totalAxles));
                        _poweredAxles = Mathf.Max(1, EditorGUILayout.IntField("Powered Axles", _poweredAxles));
                        break;
                }

                EditorGUILayout.Space();

                _powerRating = EditorGUILayout.Popup("Power Rating", _powerRating, s_powerRatingOptions);
                _decade = EditorGUILayout.Popup("Decade", _decade, s_decadeOptions);
                _unique = Mathf.Clamp(EditorGUILayout.IntField("Unique Designation", _unique), 0, 9);

                EditorGUILayout.Space();

                _capacity = EditorGUILayout.Popup("Capacity", _capacity, s_capacityOptions);
                _isSlug = EditorGUILayout.Toggle("Is Slug", _isSlug);
                _multipleWorking = EditorGUILayout.Popup("Multiple Working", _multipleWorking, s_muOptions);

                EditorGUILayout.Space();

                EditorGUILayout.TextField("Full Name", GetFullName());
                EditorGUILayout.TextField("Short Name", GetShortName());

                EditorGUILayout.Space();

                if (GUILayout.Button("Open Wiki"))
                {
                    Application.OpenURL("https://github.com/derail-valley-modding/custom-car-loader/wiki/Naming");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        public string GetFullName()
        {
            var sb = new StringBuilder();

            switch (_powertrain)
            {
                case Powertrain.S:
                    sb.Append('S');
                    sb.Append(_filteredWhyte);
                    break;
                case Powertrain.Other:
                    sb.Append(_otherPowertrain);
                    sb.Append(_poweredAxles);
                    break;
                default:
                    sb.Append(_powertrain);
                    sb.Append(_poweredAxles);
                    break;
            }

            switch (_capacity)
            {
                case 1:
                    sb.Append('P');
                    break;
                case 2:
                    sb.Append('F');
                    break;
                case 3:
                    sb.Append('U');
                    break;
                default:
                    break;
            }

            sb.Append('-');
            sb.Append(_powerRating);
            sb.Append(_decade);
            sb.Append(_unique);

            if (_isSlug)
            {
                sb.Append('S');
            }
            else if (_multipleWorking > 0)
            {
                sb.Append((char)('A' + _multipleWorking - 1));
            }

            return sb.ToString();
        }

        public string GetShortName()
        {
            var sb = new StringBuilder();

            switch (_powertrain)
            {
                case Powertrain.S:
                    sb.Append('S');
                    sb.Append(_filteredWhyte);
                    break;
                case Powertrain.Other:
                    sb.Append(_otherPowertrain);
                    sb.Append(_poweredAxles);
                    break;
                default:
                    sb.Append(_powertrain);
                    sb.Append(_poweredAxles);
                    break;
            }

            switch (_capacity)
            {
                case 1:
                    sb.Append('P');
                    break;
                case 2:
                    sb.Append('F');
                    break;
                case 3:
                    sb.Append('U');
                    break;
                default:
                    break;
            }

            if (_isSlug)
            {
                sb.Append('S');
            }
            else if (_multipleWorking > 0)
            {
                sb.Append((char)('A' + _multipleWorking - 1));
            }

            return sb.ToString();
        }
    }
}
