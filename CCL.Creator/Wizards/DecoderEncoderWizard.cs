using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static CCL.Types.Proxies.Ports.MultiplePortDecoderEncoderDefinitionProxy;

namespace CCL.Creator.Wizards
{
    internal class DecoderEncoderWizard : EditorWindow
    {
        private enum State
        {
            Setup,
            Create,
            Recover,
        }

        private static DecoderEncoderWizard? s_instance;
        private static float s_width = EditorGUIUtility.singleLineHeight + 2;
        private static GUILayoutOption s_widthOption = GUILayout.Width(s_width);

        public static void ShowWindow(MultiplePortDecoderEncoderDefinitionProxy definition)
        {
            s_instance = GetWindow<DecoderEncoderWizard>();
            s_instance.titleContent = new GUIContent("CCL - Decoder/Encoder Wizard");
            s_instance.Show();

            s_instance._definition = definition;
        }

        private MultiplePortDecoderEncoderDefinitionProxy? _definition;
        // GUI stuff.
        private Vector2 _scroll;
        private State _state;
        private bool _recoveryCalculated;
        // User options.
        private int _combinations = 7;
        private int _inputCount = 2;
        private string[] _names = new string[0];
        private int[] _limits = new int[0];
        private List<int[]> _values = new List<int[]>();

        private void OnGUI()
        {
            _definition = EditorHelpers.ObjectField("Definition", _definition, true);

            switch (_state)
            {
                case State.Create:
                    ConfigGUI();
                    break;
                case State.Recover:
                    RecoverGUI();
                    break;
                default:
                    StartGUI();
                    break;
            }
        }

        #region Setup Mode

        private void StartGUI()
        {
            _inputCount = Mathf.Max(1, EditorGUILayout.IntField("Number of Inputs", _inputCount));
            _combinations = Mathf.Max(1, EditorGUILayout.IntField("Combinations", _combinations));

            if (GUILayout.Button("Confirm"))
            {
                _state = State.Create;
                _limits = new int[_inputCount + 1];
                _values = new List<int[]>();

                for (int i = 0; i < _combinations; i++)
                {
                    int[] array = new int[_inputCount + 1];
                    array[_inputCount] = i;
                    _values.Add(array);
                }

                _names = new string[_inputCount + 1];
            }

            EditorHelpers.DrawHeader("Presets");

            if (GUILayout.Button("Steamer Headlights"))
            {
                _state = State.Create;
                CreateSteamerHeadlights();
            }

            if (GUILayout.Button("DM3 Headlights"))
            {
                _state = State.Create;
                CreateDM3Headlights();
            }

            if (GUILayout.Button("BE2 Headlights"))
            {
                _state = State.Create;
                CreateBE2Headlights();
            }

            if (GUILayout.Button("DM1U Inside Lights"))
            {
                _state = State.Create;
                CreateDM1UInsideLights();
            }
        }

        private void CreateSteamerHeadlights()
        {
            _inputCount = 2;

            _limits = new[] { 6, 6, 7 };

            _values = new List<int[]>
            {
                new[] { 0, 5, 0 },
                new[] { 0, 4, 1 },
                new[] { 1, 3, 2 },
                new[] { 2, 2, 3 },
                new[] { 3, 1, 4 },
                new[] { 4, 0, 5 },
                new[] { 5, 0, 6 },

                new[] { 2, 5, 0 },
                new[] { 2, 4, 1 },
                new[] { 2, 3, 2 },
                new[] { 2, 1, 3 },
                new[] { 2, 0, 3 },

                new[] { 5, 2, 6 },
                new[] { 4, 2, 5 },
                new[] { 3, 2, 4 },
                new[] { 1, 2, 3 },
                new[] { 0, 2, 3 },

                new[] { 5, 5, 3 },
                new[] { 0, 0, 3 }
            };

            _combinations = _values.Count;

            _names = new[]
            {
                "FRONT_HEADLIGHTS_EXT_IN",
                "REAR_HEADLIGHTS_EXT_IN",
                "HEADLIGHTS_EXT_IN"
            };
        }

        private void CreateDM3Headlights()
        {
            _inputCount = 3;

            _limits = new[] { 2, 2, 2, 6 };

            _values = new List<int[]>
            {
                new[] { 1, 0, 0, 2 },
                new[] { 1, 0, 1, 1 },
                new[] { 1, 1, 0, 1 },
                new[] { 1, 1, 1, 0 },
                new[] { 0, 0, 0, 2 },
                new[] { 0, 0, 1, 3 },
                new[] { 0, 1, 0, 4 },
                new[] { 0, 1, 1, 5 }
            };

            _combinations = _values.Count;

            _names = new[]
            {
                "FRONT_COLOR_EXT_IN",
                "FRONT_BEAM_EXT_IN",
                "FRONT_DIM_EXT_IN",
                "FRONT_EXT_IN_OUT"
            };
        }

        private void CreateBE2Headlights()
        {
            _inputCount = 2;

            _limits = new[] { 2, 2, 4 };

            _values = new List<int[]>
            {
                new[] { 0, 0, 0 },
                new[] { 1, 0, 1 },
                new[] { 0, 1, 2 },
                new[] { 1, 1, 3 },
            };

            _combinations = _values.Count;

            _names = new[]
            {
                "FRONT_HEADLIGHTS_EXT_IN",
                "REAR_HEADLIGHTS_EXT_IN",
                "HEADLIGHTS_EXT_IN"
            };
        }

        private void CreateDM1UInsideLights()
        {
            _inputCount = 2;

            _limits = new[] { 2, 2, 3 };

            _values = new List<int[]>
            {
                new[] { 0, 0, 0 },
                new[] { 1, 0, 1 },
                new[] { 1, 1, 2 },
            };

            _combinations = _values.Count;

            _names = new[]
            {
                "DASH_LIGHT_CONTROL",
                "CAB_LIGHT_CONTROL",
                "CAB_LIGHT_CONTROL_EXT_IN"
            };
        }

        #endregion

        #region Create Mode

        private void ConfigGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawNames();
            DrawLimits();
            DrawValues();

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Combination"))
            {
                // Copy the last entry.
                var array = new int[_inputCount + 1];
                var last = _values[_values.Count - 1];
                last.CopyTo(array, 0);

                _values.Add(array);
                _combinations++;
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Apply To Definition"))
            {
                Apply();
            }

            if (GUILayout.Button("Return"))
            {
                _state = State.Setup;
            }
        }

        private void DrawNames()
        {
            EditorHelpers.DrawHeader("Port IDs");
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < _inputCount; i++)
            {
                _names[i] = EditorGUILayout.TextField(_names[i]);
            }

            EditorGUILayout.Space();
            _names[_inputCount] = EditorGUILayout.TextField(_names[_inputCount]);

            EditorGUILayout.Space(s_width);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLimits()
        {
            EditorHelpers.DrawHeader("Value Counts");
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < _inputCount; i++)
            {
                _limits[i] = Mathf.Max(2, EditorGUILayout.DelayedIntField(_limits[i]));
            }

            EditorGUILayout.Space();
            _limits[_inputCount] = Mathf.Max(2, EditorGUILayout.DelayedIntField(_limits[_inputCount]));

            EditorGUILayout.Space(s_width);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawValues()
        {
            EditorHelpers.DrawHeader("Values");
            for (int i = 0; i < _combinations; i++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < _inputCount; j++)
                {
                    _values[i][j] = PopupThing(_values[i][j], _limits[j] - 1);
                }

                EditorGUILayout.Space();
                _values[i][_inputCount] = PopupThing(_values[i][_inputCount], _limits[_inputCount] - 1);

                using (new GUIColorScope(newBackground: EditorHelpers.Colors.DELETE_ACTION))
                {
                    if (GUILayout.Button("x", s_widthOption))
                    {
                        _combinations--;
                        _values.RemoveAt(i);
                        i--;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            static int PopupThing(int input, int max)
            {
                return EditorGUILayout.Popup(Mathf.Clamp(input, 0, max), Options(max));
            }

            static string[] Options(int count)
            {
                string[] options = new string[count + 1];

                for (int i = 0; i <= count; i++)
                {
                    options[i] = i.ToString();
                }

                return options;
            }
        }

        private void Apply()
        {
            if (_definition == null) return;

            Undo.RecordObject(_definition, "Overwrite DecoderEncoderDefinition With Wizard");

            _definition.values = new MultiplePortDecoderEncoderDefinitionProxy.FloatArray[_combinations];

            for (int i = 0; i < _combinations; i++)
            {
                var normalised = new float[_inputCount + 1];

                for (int j = 0; j <= _inputCount; j++)
                {
                    normalised[j] = _values[i][j] / (_limits[j] - 1.0f);
                }

                _definition.values[i] = new MultiplePortDecoderEncoderDefinitionProxy.FloatArray(normalised);
            }

            _definition.combinations = _combinations;
            _definition.outputPort = new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, _names[_inputCount]);
            _definition.inputPorts = new PortDefinition[_inputCount];

            for (int i = 0; i < _inputCount; i++)
            {
                _definition.inputPorts[i] = new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, _names[i]);
            }

            AssetHelper.SaveAsset(_definition);
            Undo.IncrementCurrentGroup();
        }

        #endregion

        #region Recover Mode

        private void RecoverGUI()
        {
            EditorGUILayout.LabelField("Coming Soon");
            return;

            GUI.enabled = _definition != null;

            if (GUILayout.Button("Calculate") && _definition != null)
            {
                _inputCount = GuessInputCount(_definition.values);
                _recoveryCalculated = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Number of Inputs", _inputCount.ToString());
            EditorGUILayout.LabelField("Combinations", _combinations.ToString());

            GUI.enabled = _recoveryCalculated;

            if (GUILayout.Button("Recover"))
            {
                _recoveryCalculated = false;
                DoRecovery();
            }

            GUI.enabled = true;
        }

        private void DoRecovery()
        {
            if (_definition == null) return;

            _state = State.Create;
            _limits = new int[_inputCount + 1];
            _values = new List<int[]>();
            _combinations = _definition.combinations;

            for (int i = 0; i < _definition.values.Length; i++)
            {
                var item = _definition.values[i];
                var array = new int[_inputCount];
                var total = GuessTotalValue(GetValuesAt(i));

                for (int j = 0; j < _inputCount && j < item.array.Length - 1; j++)
                {
                    array[j] = Mathf.RoundToInt(item.array[j] * total);
                }

                _values.Add(array);

                IEnumerable<float> GetValuesAt(int index)
                {
                    yield return 0;
                    //foreach (var item in collection)
                    //{

                    //}
                }
            }

            _names = new string[_inputCount + 1];
        }

        private static int GuessInputCount(FloatArray[] arrays)
        {
            return Mathf.Max(3, arrays.Select(x => x.array.Length).Min()) - 1;
        }

        private static int GuessTotalValue(IEnumerable<float> values)
        {
            var min = 1.0f;

            foreach (var value in values)
            {
                if (value < min && value != 0)
                {
                    min = value;
                }
            }

            var guess = Mathf.RoundToInt(1.0f / min) + 1;
            return guess;

            //return values.Distinct().Count();
        }

        #endregion
    }
}
