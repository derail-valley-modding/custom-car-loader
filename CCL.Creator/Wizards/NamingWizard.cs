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

        private static readonly Powertrain[] s_randomisePowertrainOptions = new[]
        {
            Powertrain.DE,
            Powertrain.DE,
            Powertrain.DE,
            Powertrain.DE,
            Powertrain.DE,
            Powertrain.DH,
            Powertrain.DH,
            Powertrain.DM,
            Powertrain.S,
            Powertrain.S,
            Powertrain.S,
            Powertrain.S,
            Powertrain.S,
            Powertrain.WE,
            Powertrain.WE,
            Powertrain.WE,
            Powertrain.WE,
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
        private bool _randomisePowertrain = false;

        private void OnEnable()
        {
            _otherPowertrain = string.Empty;
            _whyteInput = string.Empty;
            _unique = 0;
            _isSlug = false;
            _multipleWorking = 0;

            switch (Random.Range(0, 14))
            {
                // DE2-480
                case 0:
                    _powertrain = Powertrain.DE;
                    _poweredAxles = 2;
                    _totalAxles = 2;
                    _capacity = 0;
                    _powerRating = 4;
                    _decade = 8;
                    break;
                // DE6-860
                case 1:
                    _powertrain = Powertrain.DE;
                    _poweredAxles = 6;
                    _totalAxles = 6;
                    _capacity = 0;
                    _powerRating = 8;
                    _decade = 6;
                    break;
                // DE6-860S
                case 2:
                    _powertrain = Powertrain.DE;
                    _poweredAxles = 6;
                    _totalAxles = 6;
                    _capacity = 0;
                    _powerRating = 8;
                    _decade = 6;
                    _isSlug = true;
                    break;
                // DH4-670
                case 3:
                    _powertrain = Powertrain.DH;
                    _poweredAxles = 4;
                    _totalAxles = 4;
                    _capacity = 0;
                    _powerRating = 6;
                    _decade = 7;
                    break;
                // DH6-880
                case 4:
                    _powertrain = Powertrain.DH;
                    _poweredAxles = 6;
                    _totalAxles = 6;
                    _capacity = 0;
                    _powerRating = 8;
                    _decade = 8;
                    break;
                // DM3-540
                case 5:
                    _powertrain = Powertrain.DM;
                    _poweredAxles = 3;
                    _totalAxles = 3;
                    _capacity = 0;
                    _powerRating = 5;
                    _decade = 4;
                    break;
                // DM1U-150
                case 6:
                    _powertrain = Powertrain.DM;
                    _poweredAxles = 1;
                    _totalAxles = 2;
                    _capacity = 3;
                    _powerRating = 1;
                    _decade = 5;
                    break;
                // S060-440
                case 8:
                    _powertrain = Powertrain.S;
                    _whyteInput = "0-6-0";
                    _poweredAxles = 3;
                    _totalAxles = 3;
                    _capacity = 0;
                    _powerRating = 4;
                    _decade = 4;
                    break;
                // S282-730A
                case 9:
                    _powertrain = Powertrain.S;
                    _whyteInput = "2-8-2";
                    _poweredAxles = 4;
                    _totalAxles = 6;
                    _capacity = 0;
                    _powerRating = 7;
                    _decade = 3;
                    _multipleWorking = 1;
                    break;
                // S282-730B
                case 10:
                    _powertrain = Powertrain.S;
                    _whyteInput = "2-8-2";
                    _poweredAxles = 4;
                    _totalAxles = 6;
                    _capacity = 0;
                    _powerRating = 7;
                    _decade = 3;
                    _multipleWorking = 2;
                    break;
                // WE6-960
                case 11:
                    _powertrain = Powertrain.WE;
                    _poweredAxles = 6;
                    _totalAxles = 6;
                    _capacity = 0;
                    _powerRating = 9;
                    _decade = 6;
                    break;
                // BE2-260
                case 12:
                    _powertrain = Powertrain.BE;
                    _poweredAxles = 2;
                    _totalAxles = 2;
                    _capacity = 0;
                    _powerRating = 2;
                    _decade = 6;
                    break;
                // H1-020
                case 13:
                    _powertrain = Powertrain.H;
                    _poweredAxles = 1;
                    _totalAxles = 2;
                    _capacity = 0;
                    _powerRating = 0;
                    _decade = 2;
                    break;
                // Just cook something up.
                default:
                    Randomise(true);
                    break;
            }

            _filteredWhyte = FilterWhyte(_whyteInput);
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(new Vector2(0, _scroll)).y;
            EditorGUI.indentLevel++;

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
                        _filteredWhyte = FilterWhyte(_whyteInput);
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

            if (GUILayout.Button("Randomise"))
            {
                Randomise(_randomisePowertrain);
            }

            _randomisePowertrain = EditorGUILayout.Toggle("Randomise Powertrain", _randomisePowertrain);

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Wiki"))
            {
                Application.OpenURL("https://github.com/derail-valley-modding/custom-car-loader/wiki/Naming");
            }

            EditorGUI.indentLevel--;
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

        public void Randomise(bool powertrain)
        {
            _otherPowertrain = string.Empty;
            _whyteInput = string.Empty;
            _unique = 0;
            _isSlug = false;
            _multipleWorking = 0;

            if (powertrain)
            {
                _powertrain = s_randomisePowertrainOptions[Random.Range(0, s_randomisePowertrainOptions.Length)];
            }

            if (_powertrain == Powertrain.S)
            {
                var random = Random.value;
                int drivers = Random.Range(1, 6) * 2;
                int leading = Random.Range(0, 3) * 2;
                int trailing = Random.Range(0, 4) * 2;

                // 5% Garratt.
                if (random > 0.95)
                {
                    trailing = Mathf.Max(trailing - 2, 0);
                    _whyteInput = $"{leading}-{drivers}-{trailing}+{trailing}-{drivers}-{leading}";
                    _poweredAxles = drivers;
                }
                // 20% articulated.
                else if (random > 0.75)
                {
                    _whyteInput = $"{leading}-{drivers}-{drivers}-{trailing}";
                    _poweredAxles = drivers;
                }
                else
                {
                    _whyteInput = $"{leading}-{drivers}-{trailing}";
                    _poweredAxles = drivers / 2;
                }

                // 33% tank engine.
                _multipleWorking = Random.Range(0, 3);
            }
            else
            {
                var random = Random.value;
                int extra = 0;
                _poweredAxles = Random.Range(1, IsBigElectric() ? 7 : 5) * 2;

                if (random > 0.7)
                {
                    _poweredAxles--;
                }

                random = Random.value;

                // 2.5% 1 extra axle, 25% 2 extra axles.
                if (random > 0.725f) extra += 2;
                if (random > 0.975f) extra--;

                // At least 2 axles.
                _totalAxles = Mathf.Max(2, _poweredAxles + extra);
                _multipleWorking = Random.value > 0.6 ? Random.Range(0, 5) : 0;
            }

            _capacity = 0;
            _powerRating = Random.Range(_poweredAxles > 4 ? 2 : 0, 10);
            _decade = Random.Range(2, 10);

            _filteredWhyte = FilterWhyte(_whyteInput);

            bool IsBigElectric() => _powertrain switch
            {
                Powertrain.DE => true,
                Powertrain.WE => true,
                _ => false,
            };
        }

        public static string FilterWhyte(string whyte)
        {
            return new string(whyte.Where(char.IsDigit).ToArray());
        }
    }
}
